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
        private readonly IValidationService validationService;
        private readonly ILogger<PlaceService> logger;
        public PlaceService(IRowDataAccess rowDataAccess, 
            IPlaceDataAccess placeDataAccess, 
            ILogger<PlaceService> logger, 
            IAccountService accountService, 
            IValidationService validationService)
        {
            this.rowDataAccess = rowDataAccess;
            this.placeDataAccess = placeDataAccess;
            this.accountService = accountService;
            this.validationService = validationService;
            this.logger = logger;
        }
        public async Task CreateAsync(PlaceCreate placeCreate)
        {
            var currentUser = await accountService.GetAccountAsync();
            validationService.ValidationRequestIsNull(placeCreate);
            validationService.ValidationCannotBeNullOrNegative(placeCreate, nameof(placeCreate.RowId), placeCreate.RowId);
            validationService.ValidationCannotBeNullOrNegative(placeCreate, nameof(placeCreate.Number), placeCreate.Number);
            validationService.ValidationCannotBeNullOrNegative(placeCreate, nameof(placeCreate.Capacity), placeCreate.Capacity);
            var rowFromDB = await rowDataAccess.GetRowAsync(placeCreate.RowId);
            validationService.ValidationNotFound(rowFromDB, placeCreate.RowId);
            var rowsNumbers = rowFromDB.Places.Select(x => x.Number).ToList();
            if (rowsNumbers != null && rowsNumbers.Contains(placeCreate.Number))
            {
                validationService.ValidationFieldValueAlreadyExist(nameof(Place), nameof(placeCreate.Number));
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
            await placeDataAccess.CommitAsync();
        }
        public async Task UpdateAsync(PlaceUpdate placeUpdate)
        {
            var currentUser = await accountService.GetAccountAsync();
            validationService.ValidationRequestIsNull(placeUpdate);
            validationService.ValidationCannotBeNullOrNegative(placeUpdate, nameof(placeUpdate.Id), placeUpdate.Id);
            validationService.ValidationCannotBeNullOrNegative(placeUpdate, nameof(placeUpdate.RowId), placeUpdate.RowId);
            validationService.ValidationCannotBeNullOrNegative(placeUpdate, nameof(placeUpdate.Number), placeUpdate.Number);
            validationService.ValidationCannotBeNullOrNegative(placeUpdate, nameof(placeUpdate.Capacity), placeUpdate.Capacity);
            var placeFromDB = await placeDataAccess.GetPlaceAsync(placeUpdate.Id);
            validationService.ValidationNotFound(placeFromDB, placeUpdate.Id);
            var rowFromDB = await rowDataAccess.GetRowAsync(placeUpdate.RowId);
            validationService.ValidationNotFound(rowFromDB, placeUpdate.RowId);
            var placeWithTheSameNumber = rowFromDB.Places.FirstOrDefault(x => x.Number == placeUpdate.Number);
            if (placeWithTheSameNumber != null && placeWithTheSameNumber.Id != placeUpdate.Id)
            {
                validationService.ValidationFieldValueAlreadyExist(nameof(Place), nameof(placeUpdate.Number));
            }
            var soldTickets = placeFromDB.Tickets.Where(x => x.IsSold == true).ToList();
            validationService.ValidationEntityHasSoldTickets(nameof(Place), soldTickets);
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
            validationService.ValidationNotFound(placeFromDB, id);
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
                placesFromDB = new List<Place>();
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
            validationService.ValidationNotFound(placeFromDB, id);
            var soldTickets = placeFromDB.Tickets.Where(x => x.IsSold).ToList();
            validationService.ValidationEntityHasSoldTickets(nameof(Place), soldTickets);
            placeDataAccess.Delete(placeFromDB);
            await placeDataAccess.CommitAsync();
        }
    }
}
