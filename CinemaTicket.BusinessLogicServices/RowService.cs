using System;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using CinemaTicket.Entities;
using CinemaTicket.BusinessLogic.Interfaces;
using CinemaTicket.DataTransferObjects.Rows;
using CinemaTicket.DataTransferObjects.Places;
using CinemaTicket.DataAccess.Interfaces;

namespace CinemaTicket.BusinessLogicServices
{
    public class RowService : IRowService
    {
        private readonly IHallDataAccess hallDataAccess;
        private readonly IRowDataAccess rowDataAccess;
        private readonly IPlaceDataAccess placeDataAccess;
        private readonly IAccountService accountService;
        private readonly ILogger<RowService> logger;
        private readonly IValidationService validationService;

        public RowService(IHallDataAccess hallDataAccess, 
            IRowDataAccess rowDataAccess, 
            IPlaceDataAccess placeDataAccess, 
            ILogger<RowService> logger, 
            IAccountService accountService, 
            IValidationService validationService)
        {
            this.hallDataAccess = hallDataAccess;
            this.rowDataAccess = rowDataAccess;
            this.placeDataAccess = placeDataAccess;
            this.accountService = accountService;
            this.logger = logger;
            this.validationService = validationService;
        }
        public async Task CreateAsync(RowCreate rowCreate)
        {
            var currentUser = await accountService.GetAccountAsync();
            validationService.ValidationRequestIsNull(rowCreate);
            validationService.ValidationCannotBeNullOrNegative(rowCreate, nameof(rowCreate.HallId), rowCreate.HallId);
            validationService.ValidationCannotBeNullOrNegative(rowCreate, nameof(rowCreate.Number), rowCreate.Number);
            validationService.ValidationCannotBeNullOrNegative(rowCreate, nameof(rowCreate.PlaceCapacity), rowCreate.PlaceCapacity);
            var hallFromDB = await hallDataAccess.GetHallAsync(rowCreate.HallId);
            validationService.ValidationNotFound(hallFromDB, rowCreate.HallId);
            var rowsNumbers = hallFromDB.Rows.Select(x => x.Number).ToList();
            if (rowsNumbers != null && rowsNumbers.Contains(rowCreate.Number))
            {
                validationService.ValidationFieldValueAlreadyExist(nameof(Row), nameof(rowCreate.Number));
            }
            var row = new Row
            {
                Number = rowCreate.Number,
                HallId = rowCreate.HallId,
                CreatedOn = DateTime.UtcNow,
                ModifiedOn = DateTime.UtcNow,
                CreatedBy = currentUser.Id,
                ModifiedBy = currentUser.Id,
            };
            await rowDataAccess.CreateAsync(row);
            if (rowCreate.PlacesNumbers != null && rowCreate.PlacesNumbers.Count > 0)
            {
                rowCreate.PlacesNumbers = rowCreate.PlacesNumbers.Distinct().ToList();
                var places = new List<Place>();
                foreach (var number in rowCreate.PlacesNumbers)
                {
                    places.Add(new Place
                    {
                        Number = number,
                        CreatedOn = DateTime.Now,
                        ModifiedOn = DateTime.Now,
                        Capacity = rowCreate.PlaceCapacity,
                        CreatedBy = currentUser.Id,
                        ModifiedBy = currentUser.Id,
                        Row = row
                    });
                }
                await placeDataAccess.CreateListAsync(places);
            }
            await rowDataAccess.CommitAsync();
        }
        public async Task UpdateAsync(RowUpdate rowUpdate)
        {
            var currentUser = await accountService.GetAccountAsync();
            validationService.ValidationRequestIsNull(rowUpdate);
            validationService.ValidationCannotBeNullOrNegative(rowUpdate, nameof(rowUpdate.Id), rowUpdate.Id);
            validationService.ValidationCannotBeNullOrNegative(rowUpdate, nameof(rowUpdate.HallId), rowUpdate.HallId);
            validationService.ValidationCannotBeNullOrNegative(rowUpdate, nameof(rowUpdate.Number), rowUpdate.Number);
            validationService.ValidationCannotBeNullOrNegative(rowUpdate, nameof(rowUpdate.PlaceCapacity), rowUpdate.PlaceCapacity);
            var hallFromDB = await hallDataAccess.GetHallAsync(rowUpdate.HallId);
            validationService.ValidationNotFound(hallFromDB, rowUpdate.HallId);
            var rowWithTheSameNumber = hallFromDB.Rows.FirstOrDefault(x => x.Number == rowUpdate.Number);
            if (rowWithTheSameNumber != null && rowWithTheSameNumber.Id != rowUpdate.Id)
            {
                validationService.ValidationFieldValueAlreadyExist(nameof(Row), nameof(rowUpdate.Number));
            }
            var rowFromDB = await rowDataAccess.GetRowAsync(rowUpdate.Id);
            validationService.ValidationNotFound(rowFromDB, rowUpdate.Id);
            var soldPlacesInRow = new List<Place>();
            if (rowFromDB.Places != null && rowFromDB.Places.Count > 0)
            {
                var placesInRow = await placeDataAccess.GetPlaceListAsync();
                placesInRow = placesInRow.Where(x => x.RowId == rowFromDB.Id).ToList();
                foreach (var place in placesInRow)
                {
                    var soldTickets = new List<Ticket>();
                    soldTickets = place.Tickets.Where(x => x.IsSold).ToList();
                    if (soldTickets.Count > 0)
                    {
                        soldPlacesInRow.Add(place);
                    }
                }
            }
            rowUpdate.PlacesNumbers = rowUpdate.PlacesNumbers.Distinct().ToList();
            if (rowUpdate.PlacesNumbers == null)
            {
                rowUpdate.PlacesNumbers = new List<int>();
            }
            var dontTouchPlaceNumbers = soldPlacesInRow.Select(x => x.Number).ToList();
            var placesNumbersFromDB = rowFromDB.Places.Select(x => x.Number).ToList();
            var removePlacesNumbers = placesNumbersFromDB.Except(rowUpdate.PlacesNumbers).ToList();
            removePlacesNumbers = removePlacesNumbers.Except(dontTouchPlaceNumbers).ToList();
            var createPlaceNumbers = rowUpdate.PlacesNumbers.Except(placesNumbersFromDB).ToList();
            if (removePlacesNumbers != null && removePlacesNumbers.Count > 0)
            {
                var removePlaces = rowFromDB.Places.Where(x => removePlacesNumbers.Contains(x.Number)).ToList();
                placeDataAccess.DeleteList(removePlaces);
            }
            if (createPlaceNumbers != null && createPlaceNumbers.Count > 0)
            {
                var createPlaces = new List<Place>();
                foreach (var number in createPlaceNumbers)
                {
                    createPlaces.Add(new Place
                    {
                        Number = number,
                        Capacity = rowUpdate.PlaceCapacity,
                        CreatedOn = DateTime.UtcNow,
                        ModifiedOn = DateTime.UtcNow,
                        CreatedBy = currentUser.Id,
                        ModifiedBy = currentUser.Id,
                        RowId = rowFromDB.Id,
                    });
                }
                await placeDataAccess.CreateListAsync(createPlaces);
            }
            rowFromDB.Number = rowUpdate.Number;
            rowFromDB.ModifiedOn = DateTime.UtcNow;
            rowFromDB.ModifiedBy = currentUser.Id;
            rowFromDB.HallId = rowUpdate.HallId;
            rowDataAccess.Update(rowFromDB);
            await rowDataAccess.CommitAsync();
        }
        public async Task<RowDetails> GetAsync(int id)
        {
            var rowFromDB = await rowDataAccess.GetRowAsync(id);
            validationService.ValidationNotFound(rowFromDB, id);
            return new RowDetails
            {
                Id = rowFromDB.Id,
                Number = rowFromDB.Number,
                CreatedOn = rowFromDB.CreatedOn,
                ModifiedOn = rowFromDB.ModifiedOn,
                HallId = rowFromDB.HallId,
                Places = rowFromDB.Places.Select(x => new PlaceListElement
                {
                    Id = x.Id,
                    Capacity = x.Capacity,
                    Number = x.Number,
                    RowId = x.RowId,
                }).ToList()
            };
        }
        public async Task<List<RowListElement>> GetListAsync()
        {
            var rowsFromDB = await rowDataAccess.GetRowListAsync();
            if (rowsFromDB == null)
            {
                rowsFromDB = new List<Row>();
            }
            return rowsFromDB.Select(x => new RowListElement
            {
                Id = x.Id,
                Number = x.Number,
                HallId = x.HallId,
            }).ToList();
        }
        public async Task DeleteAsync(int id)
        {
            var rowFromDB = await rowDataAccess.GetRowAsync(id);
            validationService.ValidationNotFound(rowFromDB, id);
            var soldPlacesInRow = new List<Place>();
            if (rowFromDB.Places != null && rowFromDB.Places.Count > 0)
            {
                var placesInRow = await placeDataAccess.GetPlaceListAsync();
                placesInRow = placesInRow.Where(x => x.RowId == rowFromDB.Id).ToList();
                foreach (var place in placesInRow)
                {
                    var soldTickets = new List<Ticket>();
                    soldTickets = place.Tickets.Where(x => x.IsSold).ToList();
                    validationService.ValidationEntityHasSoldTickets(nameof(Row), soldTickets);
                }
            }
            rowDataAccess.Delete(rowFromDB);
            await rowDataAccess.CommitAsync();
        }
    }
}
