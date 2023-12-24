using System;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using CinemaTicket.Entities;
using CinemaTicket.BusinessLogic.Interfaces;
using CinemaTicket.DataTransferObjects.Halls;
using CinemaTicket.DataTransferObjects.Rows;
using CinemaTicket.DataTransferObjects.Places;
using CinemaTicket.DataAccess.Interfaces;


namespace CinemaTicket.BusinessLogicServices
{
    public class HallService : IHallService
    {
        private readonly IHallDataAccess hallDataAccess;
        private readonly IRowDataAccess rowDataAccess;
        private readonly IPlaceDataAccess placeDataAccess;
        private readonly IAccountService accountService;
        private readonly IValidationService validationService;
        private readonly ILogger<HallService> logger;
        public HallService(
            IHallDataAccess hallDataAccess, 
            IRowDataAccess rowDataAccess, 
            IPlaceDataAccess placeDataAccess, 
            IValidationService validationService, 
            ILogger<HallService> logger, 
            IAccountService accountService)
        {
            this.hallDataAccess = hallDataAccess;
            this.rowDataAccess = rowDataAccess;
            this.placeDataAccess = placeDataAccess;
            this.accountService = accountService;
            this.validationService = validationService;
            this.logger = logger;
        }
        public async Task CreateAsync(HallCreate hallCreate)
        {
            var currentUser = await accountService.GetAccountAsync();
            validationService.ValidationRequestIsNull(hallCreate);
            if (!string.IsNullOrEmpty(hallCreate.Name) || !string.IsNullOrWhiteSpace(hallCreate.Name))
            {
                var hallFromDB = await hallDataAccess.GetHallAsync(hallCreate.Name);
                validationService.ValidationSameNameAlreadyExist(hallFromDB, hallCreate.Name);
            }
            var negativeNumbers = hallCreate.RowsNumbers.Where(x => x <= 0).ToList();
            validationService.ValidationCannotBeNullOrNegative(hallCreate, nameof(hallCreate.RowsNumbers), negativeNumbers);
            var hall = new Hall
            {
                Name = hallCreate.Name,
                CreatedOn = DateTime.UtcNow,
                ModifiedOn = DateTime.UtcNow,
                CreatedBy = currentUser.Id,
                ModifiedBy = currentUser.Id
            };
            await hallDataAccess.CreateAsync(hall);
            if (hallCreate.RowsNumbers != null && hallCreate.RowsNumbers.Count > 0)
            {
                hallCreate.RowsNumbers = hallCreate.RowsNumbers.Distinct().ToList();
                var rows = new List<Row>();
                for (int i = 0; i < hallCreate.RowsNumbers.Count; i++)
                {
                    rows.Add(new Row
                    {
                        Number = hallCreate.RowsNumbers[i],
                        CreatedOn = DateTime.Now,
                        ModifiedOn = DateTime.Now,
                        Hall = hall,
                        CreatedBy = currentUser.Id,
                        ModifiedBy = currentUser.Id
                    });
                }
                await rowDataAccess.CreateListAsync(rows);
            }
            await hallDataAccess.CommitAsync();
        }
        public async Task UpdateAsync(HallUpdate hallUpdate)
        {
            var currentUser = await accountService.GetAccountAsync();
            validationService.ValidationRequestIsNull(hallUpdate);
            validationService.ValidationCannotBeNullOrNegative(hallUpdate, nameof(hallUpdate.Id), hallUpdate.Id);
            var hallFromDB = await hallDataAccess.GetHallAsync(hallUpdate.Id);
            validationService.ValidationNotFound(hallFromDB, hallUpdate.Id);
            if (!string.IsNullOrEmpty(hallUpdate.Name) || !string.IsNullOrWhiteSpace(hallUpdate.Name))
            {
                var hallWithSameName = await hallDataAccess.GetHallAsync(hallUpdate.Name);
                validationService.ValidationSameNameAlreadyExist(hallWithSameName, hallWithSameName.Name, hallUpdate.Id, hallWithSameName.Id);
            }
            var soldPlacesInHall = new List<Place>();
            if (hallFromDB.Rows != null || hallFromDB.Rows.Count > 0)
            {
                var rowsIds = hallFromDB.Rows.Select(x => x.Id).ToList();
                var allPlaces = await placeDataAccess.GetPlaceListAsync();
                var placesInHall = allPlaces.Where(x => rowsIds.Contains(x.RowId)).ToList();
                foreach (var place in placesInHall)
                {
                    var soldTickets = place.Tickets.Where(x => x.IsSold).ToList();
                    if (soldTickets.Count > 0)
                    {
                        soldPlacesInHall.Add(place);
                    }
                }
            }
            var dontTouchRowsIds = soldPlacesInHall.Select(x => x.RowId).ToList();
            dontTouchRowsIds = dontTouchRowsIds.Distinct().ToList();
            hallUpdate.RowsNumbers = hallUpdate.RowsNumbers.Distinct().ToList();
            if (hallUpdate.RowsNumbers == null)
            {
                hallUpdate.RowsNumbers = new List<int>();
            }
            var dontTouchRowsNumbers = hallFromDB.Rows.Where(x => dontTouchRowsIds.Contains(x.Id)).Select(x => x.Number).ToList();
            var rowsNumbersFromDB = hallFromDB.Rows.Select(x => x.Number).ToList();
            var removeRowsNumbers = rowsNumbersFromDB.Except(hallUpdate.RowsNumbers).ToList();
            removeRowsNumbers = removeRowsNumbers.Except(dontTouchRowsNumbers).ToList();
            var createRowsNumbers = hallUpdate.RowsNumbers.Except(rowsNumbersFromDB).ToList();
            if (removeRowsNumbers != null && removeRowsNumbers.Count > 0)
            {
                var removeRows = hallFromDB.Rows.Where(x => removeRowsNumbers.Contains(x.Number)).ToList();
                rowDataAccess.DeleteList(removeRows);
            }
            if (createRowsNumbers != null && createRowsNumbers.Count > 0)
            {
                var createRows = new List<Row>();
                foreach (var number in createRowsNumbers)
                {
                    createRows.Add(new Row
                    {
                        Number = number,
                        CreatedOn = DateTime.UtcNow,
                        ModifiedOn = DateTime.UtcNow,
                        HallId = hallFromDB.Id,
                        CreatedBy = currentUser.Id,
                        ModifiedBy = currentUser.Id
                    });
                }
                await rowDataAccess.CreateListAsync(createRows);
            }
            hallFromDB.Name = hallUpdate.Name;
            hallFromDB.ModifiedOn = DateTime.UtcNow;
            hallFromDB.ModifiedBy = currentUser.Id;
            hallDataAccess.Update(hallFromDB);
            await hallDataAccess.CommitAsync();
        }
        public async Task<HallDetails> GetAsync(int id)
        {
            var hallFromDB = await hallDataAccess.GetHallAsync(id);
            validationService.ValidationNotFound(hallFromDB, id);
            return new HallDetails
            {
                Id = hallFromDB.Id,
                Name = hallFromDB.Name,
                CreatedOn = hallFromDB.CreatedOn,
                ModifiedOn = hallFromDB.ModifiedOn,
                Rows = hallFromDB.Rows.Select(x => new RowListElement
                {
                    Id = x.Id,
                    Number = x.Number,
                    HallId = x.HallId,
                    Places = x.Places.Select(p => new PlaceListElement
                    {
                        Id = p.Id,
                        Capacity = p.Capacity,
                        Number= p.Number,
                        RowId = p.RowId,
                    }).ToList()
                }).ToList()
            };
        }
        public async Task<List<HallListElement>> GetListAsync()
        {
            var hallsFromDB = await hallDataAccess.GetHallListAsync();
            if (hallsFromDB == null)
            {
                hallsFromDB = new List<Hall>();
            }
            return hallsFromDB.Select(x => new HallListElement
            {
                Id = x.Id,
                Name = x.Name
            }).ToList();
        }
        public async Task DeleteAsync(int id)
        {
            var hallFromDB = await hallDataAccess.GetHallAsync(id);
            validationService.ValidationNotFound(hallFromDB, id);
            var soldPlacesInHall = new List<Place>();
            if (hallFromDB.Rows != null || hallFromDB.Rows.Count > 0)
            {
                var rowsIds = hallFromDB.Rows.Select(x => x.Id).ToList();
                var allPlaces = await placeDataAccess.GetPlaceListAsync();
                var placesInHall = allPlaces.Where(x => rowsIds.Contains(x.RowId)).ToList();
                foreach (var place in placesInHall)
                {
                    var soldTickets = place.Tickets.Where(x => x.IsSold).ToList();
                    validationService.ValidationEntityHasSoldTickets(nameof(Hall), soldTickets);
                }
            }
            hallDataAccess.Delete(hallFromDB);
            await hallDataAccess.CommitAsync();
        }
    }
}
