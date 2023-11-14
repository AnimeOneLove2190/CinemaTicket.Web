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
            if (sessionWithTheSameStart != null)
            {
                throw new Exception();
            }
            var hallFromDB = await hallDataAccess.GetHallAsync(sessionCreate.HallId);
            if (hallFromDB == null || hallFromDB.Rows == null || hallFromDB.Rows.Count <= 0)
            {
                throw new Exception();
            }
            var movieFromDB = await movieDataAccess.GetMovieAsync(sessionCreate.MovieId);
            if (movieFromDB == null)
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
    }
}
