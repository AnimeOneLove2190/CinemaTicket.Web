using System;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using CinemaTicket.Entities;
using CinemaTicket.BusinessLogic.Interfaces;
using CinemaTicket.DataTransferObjects.Sessions;
using CinemaTicket.DataTransferObjects.Tickets;
using CinemaTicket.DataAccess.Interfaces;

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

        public TicketService(ISessionDataAccess sessionDataAccess, IRowDataAccess rowDataAccess, IPlaceDataAccess placeDataAccess, IMovieDataAccess movieDataAccess, ITicketDataAccess ticketDataAccess, IHallDataAccess hallDataAccess)
        {
            this.hallDataAccess = hallDataAccess;
            this.movieDataAccess = movieDataAccess;
            this.sessionDataAccess = sessionDataAccess;
            this.placeDataAccess = placeDataAccess;
            this.ticketDataAccess = ticketDataAccess;
            this.rowDataAccess = rowDataAccess;
        }
        public async Task CreateAsync(TicketCreate ticketCreate)
        {
            if (ticketCreate == null)
            {
                throw new Exception();
            }
            if (ticketCreate.Price <= 0)
            {
                throw new Exception();
            }
            var placeFromDB = await placeDataAccess.GetPlaceAsync(ticketCreate.PlaceId);
            if (placeFromDB == null)
            {
                throw new Exception();
            }
            var sessionFromDB = await sessionDataAccess.GetSessionAsync(ticketCreate.SessionId);
            if (sessionFromDB == null)
            {
                throw new Exception();
            }
            var tickeDuplicate = sessionFromDB.Tickets.FirstOrDefault(x => x.PlaceId == ticketCreate.PlaceId);
            if (tickeDuplicate != null)
            {
                throw new Exception();
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
                throw new Exception();
            }
            if (ticketUpdate.Id <= 0)
            {
                throw new Exception();
            }
            if (ticketUpdate.Price <= 0)
            {
                throw new Exception();
            }
            var placeFromDB = await placeDataAccess.GetPlaceAsync(ticketUpdate.PlaceId);
            if (placeFromDB == null)
            {
                throw new Exception();
            }
            var sessionFromDB = await sessionDataAccess.GetSessionAsync(ticketUpdate.SessionId);
            if (sessionFromDB == null)
            {
                throw new Exception();
            }
            var rowFromDB = await rowDataAccess.GetRowAsync(placeFromDB.RowId);
            if (sessionFromDB.HallId != rowFromDB.HallId)
            {
                throw new Exception();
            }
            var ticketFromDB = await ticketDataAccess.GetTicketAsync(ticketUpdate.Id);
            if (ticketFromDB == null)
            {
                throw new Exception();
            }
            if (ticketFromDB.IsSold == true)
            {
                throw new Exception();
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
                throw new Exception();
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
                throw new Exception();
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
                throw new Exception();
            }
            if (ticketFromDB.IsSold == true)
            {
                throw new Exception();
            }
            await ticketDataAccess.DeleteTicketAsync(ticketFromDB);
        }
        public async Task<List<TicketView>> GetTicketViewList(int sessionId, Nullable<bool> isSold)
        {
            var sessionFromDB = await sessionDataAccess.GetSessionAsync(sessionId);
            if (sessionFromDB == null)
            {
                throw new Exception();
            }
            var hallFromDB = await hallDataAccess.GetHallAsync(sessionFromDB.HallId);
            if (hallFromDB == null || hallFromDB.Rows == null)
            {
                throw new Exception();
            }
            var rowsInHall = hallFromDB.Rows.ToList();
            var placesInHall = new List<Place>();
            for (int i = 0; i < rowsInHall.Count; i++) //TODO Проверить, как сваггер появится
            {
                var placesInRow = rowsInHall[i].Places.ToList();
                for (int j = 0; j < placesInRow.Count; j++)
                {
                    placesInHall.Add(placesInRow[j]);
                }
            }
            var movieFromDB = await movieDataAccess.GetMovieAsync(sessionFromDB.MovieId);
            if (movieFromDB == null)
            {
                throw new Exception();
            }
            var ticketStatisticList = new List<TicketView>();
            if (isSold != null)
            {
                var soldTickets = sessionFromDB.Tickets.Where(x => x.IsSold == true).ToList();
                if (soldTickets == null)
                {
                    throw new Exception();
                }
                for (int i = 0; i < soldTickets.Count; i++)
                {
                    var placeOnTheTicket = placesInHall.FirstOrDefault(x => x.Id == soldTickets[i].PlaceId);
                    if (placeOnTheTicket == null)
                    {
                        throw new Exception();
                    }
                    var rowOnTheTicket = rowsInHall.FirstOrDefault(x => x.Id == placeOnTheTicket.RowId);
                    if (rowOnTheTicket == null)
                    {
                        throw new Exception();
                    }
                    ticketStatisticList.Add(new TicketView
                    {
                        Id = soldTickets[i].Id,
                        MovieName = movieFromDB.Name,
                        PlaceNumber = placeOnTheTicket.Number,
                        RowNumber = rowOnTheTicket.Number,
                        HallId = hallFromDB.Id,
                        IsSold = soldTickets[i].IsSold,
                        Price = soldTickets[i].Price,
                    });
                }
            }
            else
            {
                var allTickets = sessionFromDB.Tickets.ToList();
                if (allTickets == null)
                {
                    throw new Exception();
                }
                for (int i = 0; i < allTickets.Count; i++)
                {
                    var placeOnTheTicket = placesInHall.FirstOrDefault(x => x.Id == allTickets[i].PlaceId);
                    if (placeOnTheTicket == null)
                    {
                        throw new Exception();
                    }
                    var rowOnTheTicket = rowsInHall.FirstOrDefault(x => x.Id == placeOnTheTicket.RowId);
                    if (rowOnTheTicket == null)
                    {
                        throw new Exception();
                    }
                    ticketStatisticList.Add(new TicketView
                    {
                        Id = allTickets[i].Id,
                        MovieName = movieFromDB.Name,
                        PlaceNumber = placeOnTheTicket.Number,
                        RowNumber = rowOnTheTicket.Number,
                        HallId = hallFromDB.Id,
                        IsSold = allTickets[i].IsSold,
                        Price = allTickets[i].Price,
                    });
                }
            }
            return ticketStatisticList;
        }
        public async Task SellTickets(List<int> ticketsIds)
        {
            var ticketsFromDB = await ticketDataAccess.GetTicketListAsync(ticketsIds);
            if (ticketsFromDB == null)
            {
                throw new Exception();
            }
            var unsoldTickets = ticketsFromDB.Where(x => x.IsSold == false).ToList();
            if (unsoldTickets == null)
            {
                throw new Exception();
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
                throw new Exception();
            }
            var unsoldTickets = ticketsFromDB.Where(x => x.IsSold == false).ToList();
            if (unsoldTickets == null)
            {
                throw new Exception();
            }
            await ticketDataAccess.DeleteTicketListAsync(unsoldTickets);
        }
    }
}
