using System;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using CinemaTicket.Entities;
using CinemaTicket.BusinessLogic.Interfaces;
using CinemaTicket.DataTransferObjects.Sessions;
using CinemaTicket.DataTransferObjects.Tickets;
using CinemaTicket.DataAccess.Interfaces;
using CinemaTicket.Infrastructure.Constants;
using CinemaTicket.Infrastructure.Exceptions;

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
        private readonly ILogger<TicketService> logger;
        public TicketService(ISessionDataAccess sessionDataAccess,
            IRowDataAccess rowDataAccess, 
            IPlaceDataAccess placeDataAccess,
            IMovieDataAccess movieDataAccess, 
            ITicketDataAccess ticketDataAccess, 
            IHallDataAccess hallDataAccess,
            ILogger<TicketService> logger)
        {
            this.hallDataAccess = hallDataAccess;
            this.movieDataAccess = movieDataAccess;
            this.sessionDataAccess = sessionDataAccess;
            this.placeDataAccess = placeDataAccess;
            this.ticketDataAccess = ticketDataAccess;
            this.rowDataAccess = rowDataAccess;
            this.logger = logger;
        }
        public async Task CreateAsync(TicketCreate ticketCreate)
        {
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
                PlaceId = ticketCreate.PlaceId,
                SessionId = ticketCreate.SessionId,
            };
            await ticketDataAccess.CreateAsync(ticket);
        }
        public async Task UpdateAsync(TicketUpdate ticketUpdate)
        {
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
            if (ticketFromDB.IsSold == true)
            {
                var exceptionMessage = string.Format(ExceptionMessageTemplate.TicketIsSold, ticketUpdate.Id);
                logger.LogError(exceptionMessage);
                throw new CustomException(exceptionMessage);
            }
            ticketFromDB.IsSold = ticketUpdate.IsSold;
            if (ticketUpdate.IsSold == true)
            {
                ticketFromDB.DateOfSale = DateTime.UtcNow;
            }
            ticketFromDB.Price = ticketUpdate.Price;
            ticketFromDB.ModifiedOn = DateTime.UtcNow;
            ticketFromDB.PlaceId = ticketUpdate.PlaceId;
            ticketFromDB.SessionId = ticketUpdate.SessionId;
            await ticketDataAccess.UpdateTicketAsync(ticketFromDB);
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
                var exceptionMessage = string.Format(ExceptionMessageTemplate.ListNotFound, nameof(Ticket));
                logger.LogError(exceptionMessage);
                throw new NotFoundException(exceptionMessage);
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
            if (ticketFromDB.IsSold == true)
            {
                throw new Exception();
            }
            await ticketDataAccess.DeleteTicketAsync(ticketFromDB);
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
            for (int i = 0; i < ticketsToUpdate.Count; i++)
            {
                ticketsToUpdate[i].IsSold = true;
            }
            await ticketDataAccess.UpdateTicketListAsync(ticketsToUpdate);
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
            var unsoldTickets = ticketsFromDB.Where(x => x.IsSold == false).ToList();
            if (unsoldTickets.Count != ticketsIds.Count)
            {
                var exceptionMessage = ExceptionMessageTemplate.TicketsAreSold;
                logger.LogError(exceptionMessage);
                throw new CustomException(exceptionMessage);
            }
            await ticketDataAccess.DeleteTicketListAsync(unsoldTickets);
        }
    }
}
