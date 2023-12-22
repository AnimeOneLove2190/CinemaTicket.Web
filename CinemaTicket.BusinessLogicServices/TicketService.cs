using System;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using CinemaTicket.Entities;
using CinemaTicket.BusinessLogic.Interfaces;
using CinemaTicket.DataTransferObjects.Sessions;
using CinemaTicket.DataTransferObjects.Tickets;
using CinemaTicket.DataAccess.Interfaces;
using CinemaTicket.Infrastructure.Constants;
using CinemaTicket.Infrastructure.Exceptions;
using OfficeOpenXml;

namespace CinemaTicket.BusinessLogicServices
{
    public class TicketService : ITicketService
    {
        private readonly IHallDataAccess hallDataAccess;
        private readonly IMovieDataAccess movieDataAccess;
        private readonly ISessionDataAccess sessionDataAccess;
        private readonly IPlaceDataAccess placeDataAccess;
        private readonly ITicketDataAccess ticketDataAccess;
        private readonly IRowDataAccess rowDataAccess;
        private readonly IAccountService accountService;
        private readonly IExcelService excelService;
        private readonly ILogger<TicketService> logger;
        private readonly IValidationService validationService;
        public TicketService(ISessionDataAccess sessionDataAccess,
            IRowDataAccess rowDataAccess, 
            IPlaceDataAccess placeDataAccess,
            IMovieDataAccess movieDataAccess, 
            ITicketDataAccess ticketDataAccess, 
            IHallDataAccess hallDataAccess,
            IAccountService accountService,
            IExcelService excelService,
            ILogger<TicketService> logger,
            IValidationService validationService)
        {
            this.hallDataAccess = hallDataAccess;
            this.movieDataAccess = movieDataAccess;
            this.sessionDataAccess = sessionDataAccess;
            this.placeDataAccess = placeDataAccess;
            this.ticketDataAccess = ticketDataAccess;
            this.rowDataAccess = rowDataAccess;
            this.accountService = accountService;
            this.excelService = excelService;
            this.logger = logger;
            this.validationService = validationService;
        }
        public async Task CreateAsync(TicketCreate ticketCreate)
        {
            var currentUser = await accountService.GetAccountAsync();
            validationService.ValidationRequestIsNull(ticketCreate);
            validationService.ValidationCannotBeNullOrNegative(ticketCreate, nameof(ticketCreate.Price), ticketCreate.Price);
            var placeFromDB = await placeDataAccess.GetPlaceAsync(ticketCreate.PlaceId);
            validationService.ValidationNotFound(placeFromDB, ticketCreate.PlaceId);
            var sessionFromDB = await sessionDataAccess.GetSessionAsync(ticketCreate.SessionId);
            validationService.ValidationNotFound(sessionFromDB, ticketCreate.SessionId);
            var tickeDuplicate = sessionFromDB.Tickets.FirstOrDefault(x => x.PlaceId == ticketCreate.PlaceId);
            if (tickeDuplicate != null)
            {
                validationService.ValidationFieldValueAlreadyExist(nameof(Ticket), nameof(ticketCreate.PlaceId));
            }
            var ticket = new Ticket
            {
                IsSold = false,
                DateOfSale = null,
                Price = ticketCreate.Price,
                CreatedOn = DateTime.UtcNow,
                ModifiedOn = DateTime.UtcNow,
                CreatedBy = currentUser.Id,
                ModifiedBy = currentUser.Id,
                PlaceId = ticketCreate.PlaceId,
                SessionId = ticketCreate.SessionId,
            };
            await ticketDataAccess.CreateAsync(ticket);
            await ticketDataAccess.CommitAsync();
        }
        public async Task UpdateAsync(TicketUpdate ticketUpdate)
        {
            var currentUser = await accountService.GetAccountAsync();
            validationService.ValidationRequestIsNull(ticketUpdate);
            validationService.ValidationCannotBeNullOrNegative(ticketUpdate, nameof(ticketUpdate.Id), ticketUpdate.Id);
            validationService.ValidationCannotBeNullOrNegative(ticketUpdate, nameof(ticketUpdate.Price), ticketUpdate.Price);
            var placeFromDB = await placeDataAccess.GetPlaceAsync(ticketUpdate.PlaceId);
            validationService.ValidationNotFound(placeFromDB, ticketUpdate.PlaceId);
            var sessionFromDB = await sessionDataAccess.GetSessionAsync(ticketUpdate.SessionId);
            validationService.ValidationNotFound(sessionFromDB, ticketUpdate.SessionId);
            var rowFromDB = await rowDataAccess.GetRowAsync(placeFromDB.RowId);
            validationService.ValidationVariousHalls(sessionFromDB.HallId, rowFromDB.HallId);
            var ticketFromDB = await ticketDataAccess.GetTicketAsync(ticketUpdate.Id);
            validationService.ValidationNotFound(ticketFromDB, ticketUpdate.Id);
            validationService.ValidationTicketIsSold(ticketFromDB.IsSold, ticketFromDB.Id);
            ticketFromDB.IsSold = ticketUpdate.IsSold;
            if (ticketUpdate.IsSold)
            {
                ticketFromDB.DateOfSale = DateTime.UtcNow;
            }
            ticketFromDB.Price = ticketUpdate.Price;
            ticketFromDB.ModifiedOn = DateTime.UtcNow;
            ticketFromDB.ModifiedBy = currentUser.Id;
            ticketFromDB.PlaceId = ticketUpdate.PlaceId;
            ticketFromDB.SessionId = ticketUpdate.SessionId;
            ticketDataAccess.Update(ticketFromDB);
            await ticketDataAccess.CommitAsync();
        }
        public async Task<TicketDetails> GetAsync(int id)
        {
            var ticketFromDB = await ticketDataAccess.GetTicketAsync(id);
            validationService.ValidationNotFound(ticketFromDB, id);
            return new TicketDetails
            {
                Id = ticketFromDB.Id,
                IsSold = ticketFromDB.IsSold, 
                DateOfSale = ticketFromDB.DateOfSale,
                Price = ticketFromDB.Price,
                CreatedOn = ticketFromDB.CreatedOn,
                ModifiedOn = ticketFromDB.ModifiedOn, 
                PlaceId = ticketFromDB.PlaceId,
                SessionId = ticketFromDB.SessionId
            };
        }
        public async Task<List<TicketListElement>> GetListAsync()
        {
            var ticketFromDB = await ticketDataAccess.GetTicketListAsync();
            if (ticketFromDB == null || ticketFromDB.Count == 0)
            {
                ticketFromDB = new List<Ticket>();
            }
            return ticketFromDB.Select(x => new TicketListElement
            {
                Id = x.Id,
                IsSold = x.IsSold,
                DateOfSale = x.DateOfSale, 
                PlaceId = x.PlaceId,
                SessionId = x.SessionId
            }).ToList();
        }
        public async Task DeleteAsync(int id)
        {
            var ticketFromDB = await ticketDataAccess.GetTicketAsync(id);
            validationService.ValidationNotFound(ticketFromDB, id);
            validationService.ValidationTicketIsSold(ticketFromDB.IsSold, ticketFromDB.Id);
            ticketDataAccess.Delete(ticketFromDB);
            await ticketDataAccess.CommitAsync();
        }
        public async Task<List<TicketView>> GetTicketViewList(int sessionId, bool? isSold)
        {
            var sessionFromDB = await sessionDataAccess.GetSessionAsync(sessionId);
            validationService.ValidationNotFound(sessionFromDB, sessionId);
            var hallFromDB = await hallDataAccess.GetHallAsync(sessionFromDB.HallId);
            validationService.ValidationNotFound(hallFromDB, sessionFromDB.HallId);
            if (sessionFromDB.Tickets == null)
            {
                return new List<TicketView>();
            }
            var rowsInHall = hallFromDB.Rows.ToList();
            var placesInHall = rowsInHall.SelectMany(x => x.Places).ToList();
            var ticketViewList = new List<TicketView>();
            if (isSold != null)
            {
                var filteredTickets = sessionFromDB.Tickets.Where(x => x.IsSold == isSold.Value).ToList();
                foreach (var ticket in filteredTickets)
                {
                    var rowOnTheTicket = rowsInHall.FirstOrDefault(x => x.Id == ticket.Place.RowId);
                    validationService.ValidationNotFound(rowOnTheTicket, ticket.Place.RowId);
                    ticketViewList.Add(new TicketView
                    {
                        Id = ticket.Id,
                        MovieName = sessionFromDB.Movie.Name,
                        PlaceNumber = ticket.Place.Number,
                        RowNumber = rowOnTheTicket.Number,
                        HallId = hallFromDB.Id,
                        IsSold = ticket.IsSold,
                        Price = ticket.Price,
                    });
                }
            }
            else
            {
                var allTickets = sessionFromDB.Tickets.ToList();
                foreach (var ticket in allTickets)
                {
                    var rowOnTheTicket = rowsInHall.FirstOrDefault(x => x.Id == ticket.Place.RowId);
                    validationService.ValidationNotFound(rowOnTheTicket, ticket.Place.RowId);
                    ticketViewList.Add(new TicketView
                    {
                        Id = ticket.Id,
                        MovieName = sessionFromDB.Movie.Name,
                        PlaceNumber = ticket.Place.Number,
                        RowNumber = rowOnTheTicket.Number,
                        HallId = hallFromDB.Id,
                        IsSold = ticket.IsSold,
                        Price = ticket.Price,
                    });
                }
            }
            return ticketViewList;
        }
        public async Task SellTickets(List<int> ticketsIds)
        {
            var currentUser = await accountService.GetAccountAsync();
            var ticketsFromDB = await ticketDataAccess.GetTicketListAsync(ticketsIds);
            validationService.ValidationListNotFound(ticketsFromDB);
            var unsoldTickets = ticketsFromDB.Where(x => x.IsSold == false).ToList();
            var unsoldTicketsIds = unsoldTickets.Select(x => x.Id).ToList();
            validationService.ValidationTicketsAreSold(unsoldTicketsIds, ticketsIds);
            var ticketsToUpdate = unsoldTickets;
            foreach (var ticket in ticketsToUpdate)
            {
                ticket.IsSold = true;
                ticket.DateOfSale = DateTime.UtcNow;
                ticket.ModifiedOn = DateTime.UtcNow;
                ticket.ModifiedBy = currentUser.Id;
                ticket.IsSold = true;
                ticket.ModifiedOn = DateTime.UtcNow;
                ticket.ModifiedBy = currentUser.Id;
            }
            ticketDataAccess.UpdateList(ticketsToUpdate);
            await ticketDataAccess.CommitAsync();

        }
        public async Task DeleteTickets(List<int> ticketsIds)
        {
            var ticketsFromDB = await ticketDataAccess.GetTicketListAsync(ticketsIds);
            validationService.ValidationListNotFound(ticketsFromDB);
            var unsoldTickets = ticketsFromDB.Where(x => !x.IsSold).ToList();
            var unsoldTicketsIds = unsoldTickets.Select(x => x.Id).ToList();
            validationService.ValidationTicketsAreSold(unsoldTicketsIds, ticketsIds);
            ticketDataAccess.DeleteList(unsoldTickets);
            await ticketDataAccess.CommitAsync();
        }
        public byte[] GetBulkTicketCreateTemplate()
        {
            return excelService.GetTemplate(BulkTicketCreateTemplate.WorksheetName, BulkTicketCreateTemplate.ColumnNames);
        }
        public async Task BulkTicketCreate(IFormFile file, int sessionId)
        {
            var currentUser = await accountService.GetAccountAsync();
            if (file == null || file.Length == 0)
            {
                var exceptionMessage = string.Format(ExceptionMessageTemplate.FieldIsRequired, nameof(file));
                logger.LogError(exceptionMessage);
                throw new CustomException(exceptionMessage);
            }
            var sessionFromDB = await sessionDataAccess.GetSessionAsync(sessionId);
            validationService.ValidationNotFound(sessionFromDB, sessionId);
            var hallFromDB = await hallDataAccess.GetHallAsync(sessionFromDB.HallId);
            validationService.ValidationNotFound(hallFromDB, sessionFromDB.HallId);

            using (var stream = new MemoryStream())
            {
                file.CopyTo(stream);
                using (var package = new ExcelPackage(stream))
                {
                    var worksheet = package.Workbook.Worksheets[0];
                    var columnIndexes = excelService.GetColumnIndexes(worksheet, BulkTicketCreateTemplate.ColumnNames);
                    var excelDataList = GetExcelDataList(worksheet, columnIndexes, sessionFromDB, hallFromDB);
                    var tickets = new List<Ticket>();
                    foreach (var ticketCreate in excelDataList)
                    {
                        tickets.Add(new Ticket
                        {
                            IsSold = false,
                            DateOfSale = null,
                            Price = ticketCreate.Price,
                            CreatedOn = DateTime.UtcNow,
                            ModifiedOn = DateTime.UtcNow,
                            CreatedBy = currentUser.Id,
                            ModifiedBy = currentUser.Id,
                            PlaceId = ticketCreate.PlaceId,
                            SessionId = ticketCreate.SessionId,
                        });
                    }
                    await ticketDataAccess.CreateListAsync(tickets);
                    await ticketDataAccess.CommitAsync();
                }
            }
        }
        private List<TicketCreate> GetExcelDataList(ExcelWorksheet worksheet, Dictionary<string, int> columnIndexes, Session session, Hall hall)
        {
            var excelDataList = new List<TicketCreate>();
            var rows = hall.Rows.ToList();
            var placeNumbers = hall.Rows.SelectMany(x => x.Places).Select(x => x.Number).ToList();
            for (int row = 2; row <= worksheet.Dimension.End.Row; row++)
            {
                var rowNumber = int.Parse(worksheet.Cells[row, columnIndexes[BulkTicketCreateTemplate.Column.RowNumber]].Value.ToString());
                var rowInHall = rows.FirstOrDefault(x => x.Number == rowNumber);
                validationService.ValidationNotFoundNumber(rowInHall, rowNumber);
                var placeNumber = int.Parse(worksheet.Cells[row, columnIndexes[BulkTicketCreateTemplate.Column.PlaceNumber]].Value.ToString());
                var placeInHall = rowInHall.Places.FirstOrDefault(x => x.Number == placeNumber);
                validationService.ValidationNotFoundNumber(placeInHall, placeNumber);
                var existTicket = session.Tickets.FirstOrDefault(x => x.PlaceId == placeInHall.Id);
                if (existTicket != null)
                {
                    validationService.ValidationFieldValueAlreadyExist(nameof(Ticket), nameof(existTicket.PlaceId));
                }
                var price = int.Parse(worksheet.Cells[row, columnIndexes[BulkTicketCreateTemplate.Column.Price]].Value.ToString());
                var stub = new Ticket();
                validationService.ValidationCannotBeNullOrNegative(stub, nameof(stub.Price), price);
                var excelData = new TicketCreate
                {
                    SessionId = session.Id,
                    PlaceId = placeInHall.Id,
                    Price = price,
                };
                excelDataList.Add(excelData);
            }
            return excelDataList;
        }
    }
}
