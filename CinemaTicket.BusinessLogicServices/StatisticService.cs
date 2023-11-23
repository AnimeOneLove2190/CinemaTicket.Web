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
    public class StatisticService
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
    }
}
