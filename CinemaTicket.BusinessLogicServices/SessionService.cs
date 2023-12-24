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
        private readonly IAccountService accountService;
        private readonly ILogger<SessionService> logger;
        private readonly IValidationService validationService;
        public SessionService(ISessionDataAccess sessionDataAccess, 
            IHallDataAccess hallDataAccess, 
            IRowDataAccess rowDataAccess, 
            IPlaceDataAccess placeDataAccess, 
            IMovieDataAccess movieDataAccess, 
            ITicketDataAccess ticketDataAccess,
            IAccountService accountService,
            ILogger<SessionService> logger, 
            IValidationService validationService)
        {
            this.sessionDataAccess = sessionDataAccess;
            this.hallDataAccess = hallDataAccess;
            this.rowDataAccess = rowDataAccess;
            this.placeDataAccess = placeDataAccess;
            this.movieDataAccess = movieDataAccess;
            this.ticketDataAccess = ticketDataAccess;
            this.accountService = accountService;
            this.logger = logger;
            this.validationService = validationService;
        }
        public async Task CreateAsync(SessionCreate sessionCreate)
        {
            var currentUser = await accountService.GetAccountAsync();
            validationService.ValidationRequestIsNull(sessionCreate);
            validationService.ValidationCannotBeNullOrNegative(sessionCreate, nameof(sessionCreate.Price), sessionCreate.Price);
            validationService.ValidationCannotBeNullOrNegative(sessionCreate, nameof(sessionCreate.Start), sessionCreate.Start);
            var hallFromDB = await hallDataAccess.GetHallAsync(sessionCreate.HallId);
            validationService.ValidationNotFound(hallFromDB, sessionCreate.HallId);
            validationService.ValidationListNotFound(hallFromDB.Rows);
            var sessionWithTheSameStart = await sessionDataAccess.GetSessionAsync(sessionCreate.Start, sessionCreate.HallId);
            if (sessionWithTheSameStart != null && sessionWithTheSameStart.HallId == sessionCreate.HallId)
            {
                validationService.ValidationFieldValueAlreadyExist(nameof(Session), nameof(sessionCreate.Start));
            }
            var movieFromDB = await movieDataAccess.GetMovieAsync(sessionCreate.MovieId);
            validationService.ValidationNotFound(movieFromDB, sessionCreate.MovieId);
            var rowsInHall = hallFromDB.Rows.ToList();
            var placesInHall = rowsInHall.SelectMany(x => x.Places).ToList();
            validationService.ValidationListNotFound(placesInHall);
            var session = new Session
            {
                Start = sessionCreate.Start,
                CreatedOn = DateTime.UtcNow,
                ModifiedOn = DateTime.UtcNow,
                MovieId = sessionCreate.MovieId,
                CreatedBy = currentUser.Id,
                ModifiedBy = currentUser.Id,
                HallId = sessionCreate.HallId,
            };
            await sessionDataAccess.CreateAsync(session);
            var ticketsInHall = new List<Ticket>();
            foreach (var place in placesInHall)
            {
                ticketsInHall.Add(new Ticket
                {
                    IsSold = false,
                    DateOfSale = null,
                    Price = sessionCreate.Price,
                    CreatedOn = DateTime.UtcNow,
                    ModifiedOn = DateTime.UtcNow,
                    CreatedBy = currentUser.Id,
                    ModifiedBy = currentUser.Id,
                    PlaceId = place.Id,
                    Session = session
                });
            }
            await ticketDataAccess.CreateListAsync(ticketsInHall);
            await ticketDataAccess.CommitAsync();
        }
        public async Task UpdateAsync(SessionUpdate sessionUpdate)
        {
            var currentUser = await accountService.GetAccountAsync();
            validationService.ValidationRequestIsNull(sessionUpdate);
            validationService.ValidationCannotBeNullOrNegative(sessionUpdate, nameof(sessionUpdate.Id), sessionUpdate.Id);
            validationService.ValidationCannotBeNullOrNegative(sessionUpdate, nameof(sessionUpdate.Price), sessionUpdate.Price);
            validationService.ValidationCannotBeNullOrNegative(sessionUpdate, nameof(sessionUpdate.Start), sessionUpdate.Start);
            var sessionFromDB = await sessionDataAccess.GetSessionAsync(sessionUpdate.Id);
            validationService.ValidationNotFound(sessionFromDB, sessionUpdate.Id);
            var soldTickets = sessionFromDB.Tickets.Where(x => x.IsSold).ToList();
            validationService.ValidationEntityHasSoldTickets(nameof(Session), soldTickets);
            var hallFromDB = await hallDataAccess.GetHallAsync(sessionUpdate.HallId);
            validationService.ValidationNotFound(hallFromDB, sessionUpdate.HallId);
            var rowsInHall = hallFromDB.Rows.ToList();
            var placesInHall = rowsInHall.SelectMany(x => x.Places).ToList();
            validationService.ValidationListNotFound(placesInHall);
            var movieFromDB = await movieDataAccess.GetMovieAsync(sessionUpdate.MovieId);
            validationService.ValidationNotFound(movieFromDB, sessionUpdate.MovieId);
            sessionFromDB.Start = sessionUpdate.Start;
            sessionFromDB.ModifiedOn = DateTime.UtcNow;
            sessionFromDB.ModifiedBy = currentUser.Id;
            sessionFromDB.MovieId = sessionUpdate.MovieId;
            sessionFromDB.HallId = sessionUpdate.HallId;
            if (sessionFromDB.HallId != sessionUpdate.HallId)
            {
                var deleteTickets = sessionFromDB.Tickets.ToList();
                ticketDataAccess.DeleteList(deleteTickets);
            }
            sessionDataAccess.Update(sessionFromDB);
            var ticketsInHall = new List<Ticket>();
            foreach (var place in placesInHall)
            {
                ticketsInHall.Add(new Ticket
                {
                    IsSold = false,
                    DateOfSale = null,
                    Price = sessionUpdate.Price,
                    CreatedOn = DateTime.UtcNow,
                    ModifiedOn = DateTime.UtcNow,
                    CreatedBy = currentUser.Id,
                    ModifiedBy = currentUser.Id,
                    PlaceId = place.Id,
                    Session = sessionFromDB
                });
            }
            await ticketDataAccess.CreateListAsync(ticketsInHall);
            await sessionDataAccess.CommitAsync();
        }
        public async Task<SessionDetails> GetAsync(int id)
        {
            var sessionFromDB = await sessionDataAccess.GetSessionAsync(id);
            validationService.ValidationNotFound(sessionFromDB, id);
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
                sessionsFromDB = new List<Session>();
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
            validationService.ValidationNotFound(sessionFromDB, id);
            var soldTicketsInSession = sessionFromDB.Tickets.Where(x => x.IsSold).ToList();
            validationService.ValidationEntityHasSoldTickets(nameof(Session), soldTicketsInSession);
            sessionDataAccess.Delete(sessionFromDB);
            await sessionDataAccess.CommitAsync();
        }
        public async Task<List<SeansView>> GetSeansViewList(DateTime? start, DateTime? end)
        {
            var startDate = start ?? DateTime.Today;
            var endDate = end ?? DateTime.Today.AddDays(1);
            var sessionsInPeriod = await sessionDataAccess.GetSessionListInPeriodAsync(startDate, endDate);
            validationService.ValidationListNotFound(sessionsInPeriod);
            var seansViewList = sessionsInPeriod.Select(session => new SeansView
            {
                Id = session.Id,
                MovieName = session.Movie.Name,
                HallId = session.HallId,
                Start = session.Start,
                Duration = session.Movie.Duration,
                HasTickets = session.Tickets != null && session.Tickets.Any(),
                HasUnsoldTickets = session != null && session.Tickets.Any(x => !x.IsSold)
            }).ToList();
            return seansViewList;
        }
    }
}
