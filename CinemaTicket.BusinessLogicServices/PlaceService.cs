using System;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using CinemaTicket.Entities;
using CinemaTicket.BusinessLogic.Interfaces;
using CinemaTicket.DataTransferObjects.Places;
using CinemaTicket.DataTransferObjects.Tickets;
using CinemaTicket.DataAccess.Interfaces;
using CinemaTicket.Infrastructure.Constants;
using CinemaTicket.Infrastructure.Exceptions;

namespace CinemaTicket.BusinessLogicServices
{
    public class PlaceService : IPlaceService
    {
        private readonly IRowDataAccess rowDataAccess;
        private readonly IPlaceDataAccess placeDataAccess;
        private readonly IAccountService accountService;
        private readonly ILogger<PlaceService> logger;
        public PlaceService(IRowDataAccess rowDataAccess, IPlaceDataAccess placeDataAccess, ILogger<PlaceService> logger, IAccountService accountService)
        {
            this.rowDataAccess = rowDataAccess;
            this.placeDataAccess = placeDataAccess;
            this.accountService = accountService;
            this.logger = logger;
        }
        public async Task CreateAsync(PlaceCreate placeCreate)
        {
            var currentUser = await accountService.GetAccountAsync();
            if (placeCreate == null)
            {
                var exceptionMessage = string.Format(ExceptionMessageTemplate.RequestIsNull, nameof(PlaceCreate));
                logger.LogError(exceptionMessage);
                throw new CustomException(exceptionMessage);
            }
            if (placeCreate.RowId <= 0)
            {
                var exceptionMessage = string.Format(ExceptionMessageTemplate.CannotBeNullOrNegatevie, nameof(PlaceCreate), nameof(placeCreate.RowId));
                logger.LogError(exceptionMessage);
                throw new CustomException(exceptionMessage);
            }
            if (placeCreate.Number <= 0)
            {
                var exceptionMessage = string.Format(ExceptionMessageTemplate.CannotBeNullOrNegatevie, nameof(PlaceCreate), nameof(placeCreate.Number));
                logger.LogError(exceptionMessage);
                throw new CustomException(exceptionMessage);
            }
            if (placeCreate.Capacity <= 0)
            {
                var exceptionMessage = string.Format(ExceptionMessageTemplate.CannotBeNullOrNegatevie, nameof(PlaceCreate), nameof(placeCreate.Capacity));
                logger.LogError(exceptionMessage);
                throw new CustomException(exceptionMessage);
            }
            var rowFromDB = await rowDataAccess.GetRowAsync(placeCreate.RowId);
            if (rowFromDB == null)
            {
                var exceptionMessage = string.Format(ExceptionMessageTemplate.NotFound, nameof(Row), placeCreate.RowId);
                logger.LogError(exceptionMessage);
                throw new NotFoundException(exceptionMessage);
            }
            var rowsNumbers = rowFromDB.Places.Select(x => x.Number).ToList();
            if (rowsNumbers != null)
            {
                if (rowsNumbers.Contains(placeCreate.Number))
                {
                    var exceptionMessage = string.Format(ExceptionMessageTemplate.SameFieldValueAlreadyExist, nameof(Place), nameof(placeCreate.Number));
                    logger.LogError(exceptionMessage);
                    throw new CustomException(exceptionMessage);
                }
            }
            var place = new Place
            {
                Number = placeCreate.Number,
                RowId = placeCreate.RowId,
                Capacity = placeCreate.Capacity,
                CreatedOn = DateTime.UtcNow,
                ModifiedOn = DateTime.UtcNow,
                CreatedBy = currentUser.Id,
                ModifiedBy = currentUser.Id
            };
            await placeDataAccess.CreateAsync(place);
        }
        public async Task UpdateAsync(PlaceUpdate placeUpdate)
        {
            var currentUser = await accountService.GetAccountAsync();
            if (placeUpdate == null)
            {
                var exceptionMessage = string.Format(ExceptionMessageTemplate.RequestIsNull, nameof(PlaceUpdate));
                logger.LogError(exceptionMessage);
                throw new CustomException(exceptionMessage);
            }
            if (placeUpdate.Id <= 0)
            {
                var exceptionMessage = string.Format(ExceptionMessageTemplate.CannotBeNullOrNegatevie, nameof(PlaceUpdate), nameof(placeUpdate.Id));
                logger.LogError(exceptionMessage);
                throw new CustomException(exceptionMessage);
            }
            if (placeUpdate.RowId <= 0)
            {
                var exceptionMessage = string.Format(ExceptionMessageTemplate.CannotBeNullOrNegatevie, nameof(PlaceUpdate), nameof(placeUpdate.RowId));
                logger.LogError(exceptionMessage);
                throw new CustomException(exceptionMessage);
            }
            if (placeUpdate.Number <= 0)
            {
                var exceptionMessage = string.Format(ExceptionMessageTemplate.CannotBeNullOrNegatevie, nameof(PlaceUpdate), nameof(placeUpdate.Number));
                logger.LogError(exceptionMessage);
                throw new CustomException(exceptionMessage);
            }
            if (placeUpdate.Capacity <= 0)
            {
                var exceptionMessage = string.Format(ExceptionMessageTemplate.CannotBeNullOrNegatevie, nameof(PlaceUpdate), nameof(placeUpdate.Capacity));
                logger.LogError(exceptionMessage);
                throw new CustomException(exceptionMessage);
            }
            var placeFromDB = await placeDataAccess.GetPlaceAsync(placeUpdate.Id);
            if (placeFromDB == null)
            {
                var exceptionMessage = string.Format(ExceptionMessageTemplate.NotFound, nameof(Place), placeUpdate.Id);
                logger.LogError(exceptionMessage);
                throw new NotFoundException(exceptionMessage);
            }
            var rowRFromDB = await rowDataAccess.GetRowAsync(placeUpdate.RowId);
            if (rowRFromDB == null)
            {
                var exceptionMessage = string.Format(ExceptionMessageTemplate.NotFound, nameof(Row), placeUpdate.RowId);
                logger.LogError(exceptionMessage);
                throw new NotFoundException(exceptionMessage);
            }
            var placeWithTheSameNumber = rowRFromDB.Places.FirstOrDefault(x => x.Number == placeUpdate.Number);
            if (placeWithTheSameNumber != null)
            {
                if (placeWithTheSameNumber.Id != placeUpdate.Id)
                {
                    var exceptionMessage = string.Format(ExceptionMessageTemplate.SameFieldValueAlreadyExist, nameof(Place), nameof(placeUpdate.Number));
                    logger.LogError(exceptionMessage);
                    throw new CustomException(exceptionMessage);
                }
            }
            var soldTickets = placeFromDB.Tickets.Where(x => x.IsSold == true).ToList();
            if (soldTickets.Count > 0)
            {
                var exceptionMessage = string.Format(ExceptionMessageTemplate.EntityHasSoldTickets, nameof(Place));
                logger.LogError(exceptionMessage);
                throw new CustomException(exceptionMessage);
            }
            placeFromDB.Number = placeUpdate.Number;
            placeFromDB.Capacity = placeUpdate.Capacity;
            placeFromDB.ModifiedOn = DateTime.UtcNow;
            placeFromDB.ModifiedBy = currentUser.Id;
            placeFromDB.RowId = placeFromDB.RowId;
            placeDataAccess.Update(placeFromDB);
            await placeDataAccess.CommitAsync();
        }
        public async Task<PlaceDetails> GetAsync(int id)
        {
            var placeFromDB = await placeDataAccess.GetPlaceAsync(id);
            if (placeFromDB == null)
            {
                var exceptionMessage = string.Format(ExceptionMessageTemplate.NotFound, nameof(Place), id);
                logger.LogError(exceptionMessage);
                throw new NotFoundException(exceptionMessage);
            }
            return new PlaceDetails
            {
                Id = placeFromDB.Id,
                Capacity = placeFromDB.Capacity,
                Number = placeFromDB.Number,
                CreatedOn = placeFromDB.CreatedOn,
                ModifiedOn = placeFromDB.ModifiedOn,
                RowId = placeFromDB.RowId,
                Tickets = placeFromDB.Tickets.Select(x => new TicketDetails
                {
                    Id = x.Id,
                    IsSold = x.IsSold,
                    DateOfSale = x.DateOfSale,
                    Price = x.Price,
                    CreatedOn = x.CreatedOn,
                    ModifiedOn = x.ModifiedOn,
                    PlaceId = x.PlaceId,
                    SessionId = x.SessionId,
                }).ToList()
            };
        }
        public async Task<List<PlaceListElement>> GetListAsync()
        {
            var placesFromDB = await placeDataAccess.GetPlaceListAsync();
            if (placesFromDB == null)
            {
                var exceptionMessage = string.Format(ExceptionMessageTemplate.ListNotFound, nameof(Place));
                logger.LogError(exceptionMessage);
                throw new NotFoundException(exceptionMessage);
            }
            return placesFromDB.Select(x => new PlaceListElement
            {
                Id = x.Id,
                Number = x.Number,
                Capacity = x.Capacity,
                RowId = x.RowId
            }).ToList();
        }
        public async Task DeleteAsync(int id)
        {
            var placeFromDB = await placeDataAccess.GetPlaceAsync(id);
            if (placeFromDB == null)
            {
                var exceptionMessage = string.Format(ExceptionMessageTemplate.NotFound, nameof(Place), id);
                logger.LogError(exceptionMessage);
                throw new NotFoundException(exceptionMessage);
            }
            var soldTickets = placeFromDB.Tickets.Where(X => X.IsSold == true).ToList();
            if (soldTickets.Count > 0)
            {
                var exceptionMessage = string.Format(ExceptionMessageTemplate.EntityHasSoldTickets, nameof(Place));
                logger.LogError(exceptionMessage);
                throw new CustomException(exceptionMessage);
            }
            placeDataAccess.Delete(placeFromDB);
            await placeDataAccess.CommitAsync();
        }
    }
}
