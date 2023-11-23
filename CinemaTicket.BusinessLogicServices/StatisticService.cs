using System;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using CinemaTicket.Entities;
using CinemaTicket.BusinessLogic.Interfaces;
using CinemaTicket.DataTransferObjects.Sessions;
using CinemaTicket.DataTransferObjects.Tickets;
using CinemaTicket.DataTransferObjects.Statistic;
using CinemaTicket.DataAccess.Interfaces;

namespace CinemaTicket.BusinessLogicServices
{
    public class StatisticService : IStatisticService
    {
        private readonly ISessionDataAccess sessionDataAccess;
        private readonly IHallDataAccess hallDataAccess;
        private readonly IRowDataAccess rowDataAccess;
        private readonly IPlaceDataAccess placeDataAccess;
        private readonly IMovieDataAccess movieDataAccess;
        private readonly ITicketDataAccess ticketDataAccess;
        public StatisticService(ISessionDataAccess sessionDataAccess, IHallDataAccess hallDataAccess, IRowDataAccess rowDataAccess, IPlaceDataAccess placeDataAccess, IMovieDataAccess movieDataAccess, ITicketDataAccess ticketDataAccess)
        {
            this.sessionDataAccess = sessionDataAccess;
            this.hallDataAccess = hallDataAccess;
            this.rowDataAccess = rowDataAccess;
            this.placeDataAccess = placeDataAccess;
            this.movieDataAccess = movieDataAccess;
            this.ticketDataAccess = ticketDataAccess;
        }
        public async Task<List<Statictic>>GetStaticticList(Nullable<DateTime> start, Nullable<DateTime> end)
        {
            var allSessions = await sessionDataAccess.GetSessionListAsync();
            var allHalls = await hallDataAccess.GetHallListAsync();
            var allMovies = await movieDataAccess.GetMovieListAsync();
            var sessionsInPeriod = new List<Session>();
            if (start != null && end != null)
            {
                sessionsInPeriod = allSessions.Where(x => x.Start >= start && x.Start <= end).ToList();
                {
                    if (sessionsInPeriod == null || sessionsInPeriod.Count <= 0)
                    {
                        throw new Exception();
                    }
                }
            }
            else
            {
                sessionsInPeriod = allSessions.Where(x => x.Start >= DateTime.Today && x.Start <= DateTime.Today).ToList();
                {
                    if (sessionsInPeriod == null || sessionsInPeriod.Count <= 0)
                    {
                        throw new Exception();
                    }
                }
            }
            var statisticList = new List<Statictic>();
            for (int i = 0; i < sessionsInPeriod.Count; i++)
            {
                var hall = allHalls.FirstOrDefault(x => x.Id == sessionsInPeriod[i].HallId);
                if (hall == null)
                {
                    throw new Exception();
                }
                var movie = allMovies.FirstOrDefault(x => x.Id == sessionsInPeriod[i].MovieId);
                if (hall == null)
                {
                    throw new Exception();
                }
                var hasTickets = false;
                var ticketsInSession = sessionsInPeriod[i].Tickets.ToList();
                if (ticketsInSession != null)
                {
                    hasTickets = true;
                }
                var hasUnsoldTickets = false;
                var usnoldTicket = sessionsInPeriod[i].Tickets.FirstOrDefault(x => x.IsSold == false);
                if (usnoldTicket != null)
                {
                    hasUnsoldTickets = true;
                }
                statisticList.Add(new Statictic
                {
                    Id = sessionsInPeriod[i].Id,
                    MovieName = movie.Name,
                    HallId = hall.Id,
                    Start = sessionsInPeriod[i].Start,
                    Duration = movie.Duration,
                    HasTickets = hasTickets,
                    HasUnsoldTickets = hasUnsoldTickets,
                });
            }
            return statisticList;
        }
        public async Task<List<TicketStatistic>> GetTicketStaticticList(int sessionId, Nullable<bool> isSold)
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
            var ticketStatisticList = new List<TicketStatistic>();
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
                    ticketStatisticList.Add(new TicketStatistic
                    {
                        Id = soldTickets[i].Id,
                        MovieName = movieFromDB.Name,
                        PlaceNumber = placeOnTheTicket.Number,
                        RowNumber = rowOnTheTicket.Number,
                        HallId = hallFromDB.Id,
                        IsSold = soldTickets[i].IsSold, //Хули нет когда да
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
                    ticketStatisticList.Add(new TicketStatistic
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
