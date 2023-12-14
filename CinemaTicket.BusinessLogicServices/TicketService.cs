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
        public TicketService(ISessionDataAccess sessionDataAccess,
            IRowDataAccess rowDataAccess, 
            IPlaceDataAccess placeDataAccess,
            IMovieDataAccess movieDataAccess, 
            ITicketDataAccess ticketDataAccess, 
            IHallDataAccess hallDataAccess,
            IAccountService accountService,
            IExcelService excelService,
            ILogger<TicketService> logger)
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
        }
        public async Task CreateAsync(TicketCreate ticketCreate)
        {
            var currentUser = await accountService.GetAccountAsync();
            if (ticketCreate == null)
            {
                var exceptionMessage = string.Format(ExceptionMessageTemplate.RequestIsNull, nameof(TicketCreate));
                logger.LogError(exceptionMessage);
                throw new CustomException(exceptionMessage);
            }
            if (ticketCreate.Price <= 0)
            {
                var exceptionMessage = string.Format(ExceptionMessageTemplate.CannotBeNullOrNegatevie, nameof(TicketCreate), nameof(ticketCreate.Price));
                logger.LogError(exceptionMessage);
                throw new CustomException(exceptionMessage);
            }
            var placeFromDB = await placeDataAccess.GetPlaceAsync(ticketCreate.PlaceId);
            if (placeFromDB == null)
            {
                var exceptionMessage = string.Format(ExceptionMessageTemplate.NotFound, nameof(Place), ticketCreate.PlaceId);
                logger.LogError(exceptionMessage);
                throw new NotFoundException(exceptionMessage);
            }
            var sessionFromDB = await sessionDataAccess.GetSessionAsync(ticketCreate.SessionId);
            if (sessionFromDB == null)
            {
                var exceptionMessage = string.Format(ExceptionMessageTemplate.NotFound, nameof(Session), ticketCreate.SessionId);
                logger.LogError(exceptionMessage);
                throw new NotFoundException(exceptionMessage);
            }
            var tickeDuplicate = sessionFromDB.Tickets.FirstOrDefault(x => x.PlaceId == ticketCreate.PlaceId);
            if (tickeDuplicate != null)
            {
                var exceptionMessage = string.Format(ExceptionMessageTemplate.SameFieldValueAlreadyExist, nameof(Place), nameof(ticketCreate.PlaceId));
                logger.LogError(exceptionMessage);
                throw new CustomException(exceptionMessage);
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
            if (ticketUpdate == null)
            {
                var exceptionMessage = string.Format(ExceptionMessageTemplate.RequestIsNull, nameof(TicketUpdate));
                logger.LogError(exceptionMessage);
                throw new CustomException(exceptionMessage);
            }
            if (ticketUpdate.Id <= 0)
            {
                var exceptionMessage = string.Format(ExceptionMessageTemplate.CannotBeNullOrNegatevie, nameof(TicketCreate), nameof(ticketUpdate.Id));
                logger.LogError(exceptionMessage);
                throw new CustomException(exceptionMessage);
            }
            if (ticketUpdate.Price <= 0)
            {
                var exceptionMessage = string.Format(ExceptionMessageTemplate.CannotBeNullOrNegatevie, nameof(TicketCreate), nameof(ticketUpdate.Price));
                logger.LogError(exceptionMessage);
                throw new CustomException(exceptionMessage);
            }
            var placeFromDB = await placeDataAccess.GetPlaceAsync(ticketUpdate.PlaceId);
            if (placeFromDB == null)
            {
                var exceptionMessage = string.Format(ExceptionMessageTemplate.NotFound, nameof(Place), ticketUpdate.PlaceId);
                logger.LogError(exceptionMessage);
                throw new NotFoundException(exceptionMessage);
            }
            var sessionFromDB = await sessionDataAccess.GetSessionAsync(ticketUpdate.SessionId);
            if (sessionFromDB == null)
            {
                var exceptionMessage = string.Format(ExceptionMessageTemplate.NotFound, nameof(Session), ticketUpdate.SessionId);
                logger.LogError(exceptionMessage);
                throw new NotFoundException(exceptionMessage);
            }
            var rowFromDB = await rowDataAccess.GetRowAsync(placeFromDB.RowId);
            if (sessionFromDB.HallId != rowFromDB.HallId)
            {
                var exceptionMessage = string.Format(ExceptionMessageTemplate.NotFound, nameof(Row), placeFromDB.RowId);
                logger.LogError(exceptionMessage);
                throw new NotFoundException(exceptionMessage);
            }
            var ticketFromDB = await ticketDataAccess.GetTicketAsync(ticketUpdate.Id);
            if (ticketFromDB == null)
            {
                var exceptionMessage = string.Format(ExceptionMessageTemplate.NotFound, nameof(Ticket), ticketUpdate.Id);
                logger.LogError(exceptionMessage);
                throw new NotFoundException(exceptionMessage);
            }
            if (ticketFromDB.IsSold)
            {
                var exceptionMessage = string.Format(ExceptionMessageTemplate.TicketIsSold, ticketUpdate.Id);
                logger.LogError(exceptionMessage);
                throw new CustomException(exceptionMessage);
            }
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
            if (ticketFromDB == null)
            {
                var exceptionMessage = string.Format(ExceptionMessageTemplate.NotFound, nameof(Ticket), id);
                logger.LogError(exceptionMessage);
                throw new NotFoundException(exceptionMessage);
            }
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
            if (ticketFromDB == null)
            {
                var exceptionMessage = string.Format(ExceptionMessageTemplate.NotFound, nameof(Ticket), id);
                logger.LogError(exceptionMessage);
                throw new NotFoundException(exceptionMessage);
            }
            if (ticketFromDB.IsSold)
            {
                var exceptionMessage = string.Format(ExceptionMessageTemplate.TicketIsSold, id);
                logger.LogError(exceptionMessage);
                throw new CustomException(exceptionMessage);
            }
            ticketDataAccess.Delete(ticketFromDB);
            await ticketDataAccess.CommitAsync();
        }
        public async Task<List<TicketView>> GetTicketViewList(int sessionId, bool? isSold)
        {
            var sessionFromDB = await sessionDataAccess.GetSessionAsync(sessionId);
            if (sessionFromDB == null)
            {
                var exceptionMessage = string.Format(ExceptionMessageTemplate.NotFound, nameof(Session), sessionId);
                logger.LogError(exceptionMessage);
                throw new NotFoundException(exceptionMessage);
            }
            var hallFromDB = await hallDataAccess.GetHallAsync(sessionFromDB.HallId);
            if (hallFromDB == null)
            {
                var exceptionMessage = string.Format(ExceptionMessageTemplate.NotFound, nameof(Hall), sessionFromDB.HallId);
                logger.LogError(exceptionMessage);
                throw new NotFoundException(exceptionMessage);
            }
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
                    if (rowOnTheTicket == null)
                    {
                        var exceptionMessage = string.Format(ExceptionMessageTemplate.NotFound, nameof(Row), ticket.Place.RowId);
                        logger.LogError(exceptionMessage);
                        throw new NotFoundException(exceptionMessage);
                    }
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
                    if (rowOnTheTicket == null)
                    {
                        var exceptionMessage = string.Format(ExceptionMessageTemplate.NotFound, nameof(Row), ticket.Place.RowId);
                        logger.LogError(exceptionMessage);
                        throw new NotFoundException(exceptionMessage);
                    }
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
            if (ticketsFromDB == null)
            {
                var exceptionMessage = string.Format(ExceptionMessageTemplate.ListNotFound, nameof(Ticket));
                logger.LogError(exceptionMessage);
                throw new NotFoundException(exceptionMessage);
            }
            var unsoldTickets = ticketsFromDB.Where(x => x.IsSold == false).ToList();
            if (unsoldTickets.Count != ticketsIds.Count)
            {
                var exceptionMessage = ExceptionMessageTemplate.TicketsAreSold;
                logger.LogError(exceptionMessage);
                throw new CustomException(exceptionMessage);
            }
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
            if (ticketsFromDB == null)
            {
                var exceptionMessage = string.Format(ExceptionMessageTemplate.ListNotFound, nameof(Ticket));
                logger.LogError(exceptionMessage);
                throw new NotFoundException(exceptionMessage);
            }
            var unsoldTickets = ticketsFromDB.Where(x => !x.IsSold).ToList();
            if (unsoldTickets.Count != ticketsIds.Count)
            {
                var exceptionMessage = ExceptionMessageTemplate.TicketsAreSold;
                logger.LogError(exceptionMessage);
                throw new CustomException(exceptionMessage);
            }
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
            if (sessionFromDB == null)
            {
                var exceptionMessage = string.Format(ExceptionMessageTemplate.NotFound, nameof(Session), sessionId);
                logger.LogError(exceptionMessage);
                throw new NotFoundException(exceptionMessage);
            }
            var hallFromDB = await hallDataAccess.GetHallAsync(sessionFromDB.HallId);
            if (hallFromDB == null)
            {
                var exceptionMessage = string.Format(ExceptionMessageTemplate.NotFound, nameof(Hall), sessionFromDB.HallId);
                logger.LogError(exceptionMessage);
                throw new NotFoundException(exceptionMessage);
            }
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
                if (rowInHall == null)
                {
                    var exceptionMessage = string.Format(ExceptionMessageTemplate.NotFoundNumber, nameof(Row), rowNumber);
                    logger.LogError(exceptionMessage);
                    throw new NotFoundException(exceptionMessage);
                }
                var placeNumber = int.Parse(worksheet.Cells[row, columnIndexes[BulkTicketCreateTemplate.Column.PlaceNumber]].Value.ToString());
                var placeInHall = rowInHall.Places.FirstOrDefault(x => x.Number == placeNumber);
                if (placeInHall == null)
                {
                    var exceptionMessage = string.Format(ExceptionMessageTemplate.NotFoundNumber, nameof(Place), placeNumber);
                    logger.LogError(exceptionMessage);
                    throw new NotFoundException(exceptionMessage);
                }
                var existTicket = session.Tickets.FirstOrDefault(x => x.PlaceId == placeInHall.Id);
                if (existTicket != null)
                {
                    var exceptionMessage = string.Format(ExceptionMessageTemplate.SameFieldValueAlreadyExist, nameof(Ticket), existTicket.PlaceId);
                    logger.LogError(exceptionMessage);
                    throw new CustomException(exceptionMessage);
                }
                var price = int.Parse(worksheet.Cells[row, columnIndexes[BulkTicketCreateTemplate.Column.Price]].Value.ToString());
                if (price <= 0)
                {
                    var exceptionMessage = string.Format(ExceptionMessageTemplate.CannotBeNullOrNegatevie, nameof(Ticket), nameof(Ticket.Price));
                    logger.LogError(exceptionMessage);
                    throw new CustomException(exceptionMessage);
                }
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
