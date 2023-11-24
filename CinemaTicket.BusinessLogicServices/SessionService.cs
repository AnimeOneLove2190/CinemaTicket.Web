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
    public class SessionService : ISessionService
    {
        private readonly ISessionDataAccess sessionDataAccess;
        private readonly IHallDataAccess hallDataAccess;
        private readonly IRowDataAccess rowDataAccess;
        private readonly IPlaceDataAccess placeDataAccess;
        private readonly IMovieDataAccess movieDataAccess;
        private readonly ITicketDataAccess ticketDataAccess;
        public SessionService(ISessionDataAccess sessionDataAccess, IHallDataAccess hallDataAccess, IRowDataAccess rowDataAccess, IPlaceDataAccess placeDataAccess, IMovieDataAccess movieDataAccess, ITicketDataAccess ticketDataAccess)
        {
            this.sessionDataAccess = sessionDataAccess;
            this.hallDataAccess = hallDataAccess;
            this.rowDataAccess = rowDataAccess;
            this.placeDataAccess = placeDataAccess;
            this.movieDataAccess = movieDataAccess;
            this.ticketDataAccess = ticketDataAccess;
        }
        public async Task CreateAsync(SessionCreate sessionCreate)
        {
            if (sessionCreate == null)
            {
                throw new Exception();
            }
            if (sessionCreate.Price <= 0)
            {
                throw new Exception();
            }
            if (sessionCreate.Start == null || sessionCreate.Start <= DateTime.Today)
            {
                throw new Exception();
            }
            var sessionWithTheSameStart = await sessionDataAccess.GetSessionAsync(sessionCreate.Start);
            if (sessionWithTheSameStart != null && sessionWithTheSameStart.HallId == sessionCreate.HallId)
            {
                throw new Exception();
            }
            var hallFromDB = await hallDataAccess.GetHallAsync(sessionCreate.HallId);
            if (hallFromDB == null || hallFromDB.Rows == null)
            {
                throw new Exception();
            }
            var movieFromDB = await movieDataAccess.GetMovieAsync(sessionCreate.MovieId);
            if (movieFromDB == null)
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
            if (placesInHall == null)
            {
                throw new Exception();
            }
            var session = new Session
            {
                Start = sessionCreate.Start,
                CreatedOn = DateTime.UtcNow,
                ModifiedOn = DateTime.UtcNow,
                MovieId = sessionCreate.MovieId,
                HallId = sessionCreate.HallId,
            };
            await sessionDataAccess.CreateAsync(session);
            var ticketsInHall = new List<Ticket>();
            for (int i = 0; i < placesInHall.Count; i++)
            {
                ticketsInHall.Add(new Ticket
                {
                    IsSold = false,
                    DateOfSale = null,
                    Price = sessionCreate.Price,
                    CreatedOn = DateTime.UtcNow,
                    ModifiedOn = DateTime.UtcNow,
                    PlaceId = placesInHall[i].Id,
                    SessionId = session.Id
                });
            }
            await ticketDataAccess.CreateAsync(ticketsInHall);
        }
        public async Task UpdateAsync(SessionUpdate sessionUpdate)
        {
            if (sessionUpdate == null)
            {
                throw new Exception();
            }
            if (sessionUpdate.Id <= 0)
            {
                throw new Exception();
            }
            if (sessionUpdate.Start < DateTime.UtcNow)
            {
                throw new Exception();
            }
            if (sessionUpdate.Price <= 0)
            {
                throw new Exception();
            }
            var sessionFromDB = await sessionDataAccess.GetSessionAsync(sessionUpdate.Id);
            if (sessionFromDB == null)
            {
                throw new Exception();
            }
            var soldTickets = sessionFromDB.Tickets.Where(x => x.IsSold == true).ToList();
            if (soldTickets.Count > 0)
            {
                throw new Exception();
            }
            var hallFromDB = await hallDataAccess.GetHallAsync(sessionUpdate.HallId);
            if (hallFromDB == null || hallFromDB.Rows == null || hallFromDB.Rows.Count <= 0)
            {
                throw new Exception();
            }
            var rowsIds = hallFromDB.Rows.Select(x => x.Id).ToList();
            var allPlaces = await placeDataAccess.GetPlaceListAsync();
            var placesInHall = allPlaces.Where(x => rowsIds.Contains(x.RowId)).ToList();
            if (placesInHall == null || placesInHall.Count <= 0)
            {
                throw new Exception();
            }
            var movieFromDB = await movieDataAccess.GetMovieAsync(sessionUpdate.MovieId);
            if (movieFromDB == null)
            {
                throw new Exception();
            }
            sessionFromDB.Start = sessionUpdate.Start;
            sessionFromDB.ModifiedOn = DateTime.UtcNow;
            sessionFromDB.MovieId = sessionUpdate.MovieId;
            sessionFromDB.HallId = sessionUpdate.HallId;
            if (sessionFromDB.HallId != sessionUpdate.HallId)
            {
                var deleteTickets = sessionFromDB.Tickets.ToList();
                await ticketDataAccess.DeleteTicketListAsync(deleteTickets);
            }
            await sessionDataAccess.CreateAsync(sessionFromDB);
            var ticketsInHall = new List<Ticket>();
            for (int i = 0; i < placesInHall.Count; i++)
            {
                ticketsInHall.Add(new Ticket
                {
                    IsSold = false,
                    DateOfSale = null,
                    Price = sessionUpdate.Price,
                    CreatedOn = DateTime.UtcNow,
                    ModifiedOn = DateTime.UtcNow,
                    PlaceId = placesInHall[i].Id,
                    SessionId = sessionFromDB.Id
                });
            }
            await ticketDataAccess.CreateAsync(ticketsInHall);
        }
        public async Task<SessionDetails> GetAsync(int id)
        {
            var sessionFromDB = await sessionDataAccess.GetSessionAsync(id);
            if (sessionFromDB == null)
            {
                throw new Exception();
            }
            return new SessionDetails
            {
                Id = sessionFromDB.Id,
                Start = sessionFromDB.Start,
                CreatedOn = sessionFromDB.CreatedOn,
                ModifiedOn = sessionFromDB.ModifiedOn,
                MovieId = sessionFromDB.MovieId,
                HallId = sessionFromDB.HallId,
                Tickets = sessionFromDB.Tickets.Select(x => new TicketDetails
                {
                    Id = x.Id,
                    IsSold = x.IsSold,
                    DateOfSale = x.DateOfSale,
                    Price = x.Price,
                    CreatedOn = x.CreatedOn,
                    ModifiedOn = x.ModifiedOn,
                    PlaceId = x.PlaceId,
                    SessionId = x.SessionId
                }).ToList()
            };
        }
        public async Task<List<SessionListElement>> GetListAsync()
        {
            var sessionsFromDB = await sessionDataAccess.GetSessionListAsync();
            if (sessionsFromDB == null || sessionsFromDB.Count == 0)
            {
                throw new Exception();
            }
            return sessionsFromDB.Select(x => new SessionListElement
            {
                Id = x.Id,
                Start = x.Start,
                MovieId = x.MovieId,
                HallId = x.HallId,
            }).ToList();
        }
        public async Task DeleteAsync(int id)
        {
            var sessionFromDB = await sessionDataAccess.GetSessionAsync(id);
            if (sessionFromDB == null)
            {
                throw new Exception();
            }
            var soldTicketsInSession = sessionFromDB.Tickets.Where(x => x.IsSold == true).ToList();
            if (soldTicketsInSession.Count > 0)
            {
                throw new Exception();
            }
            await sessionDataAccess.DeleteSessionAsync(sessionFromDB);
        }
        public async Task<List<SeansView>> GetSeansViewList(Nullable<DateTime> start, Nullable<DateTime> end)
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
            var statisticList = new List<SeansView>();
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
                statisticList.Add(new SeansView
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
