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
        public SessionService(ISessionDataAccess sessionDataAccess, 
            IHallDataAccess hallDataAccess, 
            IRowDataAccess rowDataAccess, 
            IPlaceDataAccess placeDataAccess, 
            IMovieDataAccess movieDataAccess, 
            ITicketDataAccess ticketDataAccess,
            IAccountService accountService,
            ILogger<SessionService> logger)
        {
            this.sessionDataAccess = sessionDataAccess;
            this.hallDataAccess = hallDataAccess;
            this.rowDataAccess = rowDataAccess;
            this.placeDataAccess = placeDataAccess;
            this.movieDataAccess = movieDataAccess;
            this.ticketDataAccess = ticketDataAccess;
            this.accountService = accountService;
            this.logger = logger;
        }
        public async Task CreateAsync(SessionCreate sessionCreate)
        {
            var currentUser = await accountService.GetAccountAsync();
            if (sessionCreate == null)
            {
                var exceptionMessage = string.Format(ExceptionMessageTemplate.RequestIsNull, nameof(SessionCreate));
                logger.LogError(exceptionMessage);
                throw new CustomException(exceptionMessage);
            }
            if (sessionCreate.Price <= 0)
            {
                var exceptionMessage = string.Format(ExceptionMessageTemplate.CannotBeNullOrNegatevie, nameof(SessionCreate), nameof(sessionCreate.Price));
                logger.LogError(exceptionMessage);
                throw new CustomException(exceptionMessage);
            }
            if (sessionCreate.Start == null)
            {
                var exceptionMessage = string.Format(ExceptionMessageTemplate.CannotBeNullOrNegatevie, nameof(SessionCreate), nameof(sessionCreate.Start));
                logger.LogError(exceptionMessage);
                throw new CustomException(exceptionMessage);
            }
            var hallFromDB = await hallDataAccess.GetHallAsync(sessionCreate.HallId);
            if (hallFromDB == null || hallFromDB.Rows == null)
            {
                var exceptionMessage = string.Format(ExceptionMessageTemplate.NotFound, nameof(Hall), sessionCreate.HallId);
                logger.LogError(exceptionMessage);
                throw new NotFoundException(exceptionMessage);
            }
            var sessionWithTheSameStart = await sessionDataAccess.GetSessionAsync(sessionCreate.Start, sessionCreate.HallId);
            if (sessionWithTheSameStart != null && sessionWithTheSameStart.HallId == sessionCreate.HallId)
            {
                var exceptionMessage = string.Format(ExceptionMessageTemplate.SameFieldValueAlreadyExist, nameof(Session), nameof(sessionCreate.Start));
                logger.LogError(exceptionMessage);
                throw new CustomException(exceptionMessage);
            }
            var movieFromDB = await movieDataAccess.GetMovieAsync(sessionCreate.MovieId);
            if (movieFromDB == null)
            {
                var exceptionMessage = string.Format(ExceptionMessageTemplate.NotFound, nameof(Movie), sessionCreate.MovieId);
                logger.LogError(exceptionMessage);
                throw new NotFoundException(exceptionMessage);
            }
            var rowsInHall = hallFromDB.Rows.ToList();
            var placesInHall = rowsInHall.SelectMany(x => x.Places).ToList();
            if (placesInHall == null)
            {
                var exceptionMessage = string.Format(ExceptionMessageTemplate.ListNotFound, nameof(Place));
                logger.LogError(exceptionMessage);
                throw new NotFoundException(exceptionMessage);
            }
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
            for (int i = 0; i < placesInHall.Count; i++)
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
                    PlaceId = placesInHall[i].Id,
                    Session = session
                });
            }
            await ticketDataAccess.CreateListAsync(ticketsInHall);
            await ticketDataAccess.CommitAsync();
        }
        public async Task UpdateAsync(SessionUpdate sessionUpdate)
        {
            var currentUser = await accountService.GetAccountAsync();
            if (sessionUpdate == null)
            {
                var exceptionMessage = string.Format(ExceptionMessageTemplate.RequestIsNull, nameof(SessionUpdate));
                logger.LogError(exceptionMessage);
                throw new CustomException(exceptionMessage);
            }
            if (sessionUpdate.Id <= 0)
            {
                var exceptionMessage = string.Format(ExceptionMessageTemplate.CannotBeNullOrNegatevie, nameof(SessionUpdate), nameof(sessionUpdate.Id));
                logger.LogError(exceptionMessage);
                throw new CustomException(exceptionMessage);
            }
            if (sessionUpdate.Start == null)
            {
                var exceptionMessage = string.Format(ExceptionMessageTemplate.CannotBeNullOrNegatevie, nameof(SessionUpdate), nameof(sessionUpdate.Start));
                logger.LogError(exceptionMessage);
                throw new CustomException(exceptionMessage);
            }
            if (sessionUpdate.Price <= 0)
            {
                var exceptionMessage = string.Format(ExceptionMessageTemplate.CannotBeNullOrNegatevie, nameof(SessionUpdate), nameof(sessionUpdate.Price));
                logger.LogError(exceptionMessage);
                throw new CustomException(exceptionMessage);
            }
            var sessionFromDB = await sessionDataAccess.GetSessionAsync(sessionUpdate.Id);
            if (sessionFromDB == null)
            {
                var exceptionMessage = string.Format(ExceptionMessageTemplate.NotFound, nameof(Session), sessionUpdate.Id);
                logger.LogError(exceptionMessage);
                throw new NotFoundException(exceptionMessage);
            }
            var soldTickets = sessionFromDB.Tickets.Where(x => x.IsSold == true).ToList();
            if (soldTickets.Count > 0)
            {
                var exceptionMessage = string.Format(ExceptionMessageTemplate.EntityHasSoldTickets, nameof(Session));
                logger.LogError(exceptionMessage);
                throw new CustomException(exceptionMessage);
            }
            var hallFromDB = await hallDataAccess.GetHallAsync(sessionUpdate.HallId);
            if (hallFromDB == null)
            {
                var exceptionMessage = string.Format(ExceptionMessageTemplate.NotFound, nameof(Hall), sessionUpdate.HallId);
                logger.LogError(exceptionMessage);
                throw new NotFoundException(exceptionMessage);
            }
            var rowsInHall = hallFromDB.Rows.ToList();
            var placesInHall = rowsInHall.SelectMany(x => x.Places).ToList();
            if (placesInHall == null)
            {
                var exceptionMessage = string.Format(ExceptionMessageTemplate.ListNotFound, nameof(Place));
                logger.LogError(exceptionMessage);
                throw new NotFoundException(exceptionMessage);
            }
            var movieFromDB = await movieDataAccess.GetMovieAsync(sessionUpdate.MovieId);
            if (movieFromDB == null)
            {
                var exceptionMessage = string.Format(ExceptionMessageTemplate.NotFound, nameof(Movie), sessionUpdate.MovieId);
                logger.LogError(exceptionMessage);
                throw new NotFoundException(exceptionMessage);
            }
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
            for (int i = 0; i < placesInHall.Count; i++)
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
                    PlaceId = placesInHall[i].Id,
                    Session = sessionFromDB
                });
            }
            await ticketDataAccess.CreateListAsync(ticketsInHall);
            await sessionDataAccess.CommitAsync();
        }
        public async Task<SessionDetails> GetAsync(int id)
        {
            var sessionFromDB = await sessionDataAccess.GetSessionAsync(id);
            if (sessionFromDB == null)
            {
                var exceptionMessage = string.Format(ExceptionMessageTemplate.NotFound, nameof(Session), id);
                logger.LogError(exceptionMessage);
                throw new NotFoundException(exceptionMessage);
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
                var exceptionMessage = string.Format(ExceptionMessageTemplate.ListNotFound, nameof(Session));
                logger.LogError(exceptionMessage);
                throw new NotFoundException(exceptionMessage);
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
                var exceptionMessage = string.Format(ExceptionMessageTemplate.NotFound, nameof(Session), id);
                logger.LogError(exceptionMessage);
                throw new NotFoundException(exceptionMessage);
            }
            var soldTicketsInSession = sessionFromDB.Tickets.Where(x => x.IsSold == true).ToList();
            if (soldTicketsInSession.Count > 0)
            {
                var exceptionMessage = string.Format(ExceptionMessageTemplate.EntityHasSoldTickets, nameof(Session));
                logger.LogError(exceptionMessage);
                throw new CustomException(exceptionMessage);
            }
            sessionDataAccess.Delete(sessionFromDB);
            await sessionDataAccess.CommitAsync();
        }
        public async Task<List<SeansView>> GetSeansViewList(DateTime? start, DateTime? end)
        {
            var startDate = start ?? DateTime.Today;
            var endDate = end ?? DateTime.Today.AddDays(1);
            var sessionsInPeriod = await sessionDataAccess.GetSessionListInPeriodAsync(startDate, endDate);
            if (sessionsInPeriod == null)
            {
                var exceptionMessage = string.Format(ExceptionMessageTemplate.ListNotFound, nameof(Session));
                logger.LogError(exceptionMessage);
                throw new NotFoundException(exceptionMessage);
            }
            var seansViewList = sessionsInPeriod.Select(session => new SeansView
            {
                Id = session.Id,
                MovieName = session.Movie.Name,
                HallId = session.HallId,
                Start = session.Start,
                Duration = session.Movie.Duration,
                HasTickets = session.Tickets != null && session.Tickets.Any(),
                HasUnsoldTickets = session != null && session.Tickets.Any(x => x.IsSold == false)
            }).ToList();
            return seansViewList;
        }
    }
}
