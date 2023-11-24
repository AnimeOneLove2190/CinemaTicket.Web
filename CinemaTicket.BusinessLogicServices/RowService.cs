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
using CinemaTicket.Infrastructure.Constants;
using CinemaTicket.Infrastructure.Exceptions;

namespace CinemaTicket.BusinessLogicServices
{
    public class RowService : IRowService
    {
        private readonly IHallDataAccess hallDataAccess;
        private readonly IRowDataAccess rowDataAccess;
        private readonly IPlaceDataAccess placeDataAccess;
        private readonly ILogger<RowService> logger;

        public RowService(IHallDataAccess hallDataAccess, IRowDataAccess rowDataAccess, IPlaceDataAccess placeDataAccess, ILogger<RowService> logger)
        {
            this.hallDataAccess = hallDataAccess;
            this.rowDataAccess = rowDataAccess;
            this.placeDataAccess = placeDataAccess;
            this.logger = logger;
        }
        public async Task CreateAsync(RowCreate rowCreate)
        {
            if (rowCreate == null)
            {
                var exceptionMessage = string.Format(ExceptionMessageTemplate.RequestIsNull, nameof(RowCreate));
                logger.LogError(exceptionMessage);
                throw new CustomException(exceptionMessage);
            }
            if (rowCreate.HallId <= 0)
            {
                var exceptionMessage = string.Format(ExceptionMessageTemplate.CannotBeNegatevie, nameof(RowCreate), nameof(rowCreate.HallId));
                logger.LogError(exceptionMessage);
                throw new CustomException(exceptionMessage);
            }
            if (rowCreate.Number <= 0)
            {
                var exceptionMessage = string.Format(ExceptionMessageTemplate.CannotBeNegatevie, nameof(RowCreate), nameof(rowCreate.Number));
                logger.LogError(exceptionMessage);
                throw new CustomException(exceptionMessage);
            }
            var hallFromDB = await hallDataAccess.GetHallAsync(rowCreate.HallId);
            if (hallFromDB == null)
            {
                var exceptionMessage = string.Format(ExceptionMessageTemplate.NotFound, nameof(Hall), rowCreate.HallId);
                logger.LogError(exceptionMessage);
                throw new NotFoundException(exceptionMessage);
            }
            var rowsNumbers = hallFromDB.Rows.Select(x => x.Number).ToList();
            if (rowsNumbers != null)
            {
                if (rowsNumbers.Contains(rowCreate.Number))
                {
                    var exceptionMessage = string.Format(ExceptionMessageTemplate.SameFieldValueAlreadyExist, nameof(Row), nameof(rowCreate.Number));
                    logger.LogError(exceptionMessage);
                    throw new CustomException(exceptionMessage);
                }
            }
            var row = new Row
            {
                Number = rowCreate.Number,
                HallId = rowCreate.HallId,
                CreatedOn = DateTime.UtcNow,
                ModifiedOn = DateTime.UtcNow,
            };
            await rowDataAccess.CreateAsync(row);
            if (rowCreate.PlacesNumbers != null && rowCreate.PlacesNumbers.Count > 0)
            {
                rowCreate.PlacesNumbers = rowCreate.PlacesNumbers.Distinct().ToList();
                var places = new List<Place>();
                for (int i = 0; i < rowCreate.PlacesNumbers.Count; i++)
                {
                    places.Add(new Place
                    {
                        Number = rowCreate.PlacesNumbers[i],
                        CreatedOn = DateTime.Now,
                        ModifiedOn = DateTime.Now,
                        RowId = row.Id
                    });
                }
                await placeDataAccess.CreateAsync(places);
            }
        }
        public async Task UpdateAsync(RowUpdate rowUpdate)
        {
            if (rowUpdate == null)
            {
                var exceptionMessage = string.Format(ExceptionMessageTemplate.RequestIsNull, nameof(RowUpdate));
                logger.LogError(exceptionMessage);
                throw new CustomException(exceptionMessage);
            }
            if (rowUpdate.Id <= 0)
            {
                var exceptionMessage = string.Format(ExceptionMessageTemplate.CannotBeNegatevie, nameof(RowUpdate), nameof(rowUpdate.Id));
                logger.LogError(exceptionMessage);
                throw new CustomException(exceptionMessage);
            }
            if (rowUpdate.HallId <= 0)
            {
                var exceptionMessage = string.Format(ExceptionMessageTemplate.CannotBeNegatevie, nameof(RowUpdate), nameof(rowUpdate.HallId));
                logger.LogError(exceptionMessage);
                throw new CustomException(exceptionMessage);
            }
            if (rowUpdate.Number <= 0)
            {
                var exceptionMessage = string.Format(ExceptionMessageTemplate.CannotBeNegatevie, nameof(RowUpdate), nameof(rowUpdate.Number));
                logger.LogError(exceptionMessage);
                throw new CustomException(exceptionMessage);
            }
            var hallFromDB = await hallDataAccess.GetHallAsync(rowUpdate.HallId);
            if (hallFromDB == null)
            {
                var exceptionMessage = string.Format(ExceptionMessageTemplate.NotFound, nameof(Hall), rowUpdate.HallId);
                logger.LogError(exceptionMessage);
                throw new NotFoundException(exceptionMessage);
            }
            var rowWithTheSameNumber = hallFromDB.Rows.FirstOrDefault(x => x.Number == rowUpdate.Number);
            if (rowWithTheSameNumber != null)
            {
                if (rowWithTheSameNumber.Id != rowUpdate.Id)
                {
                    var exceptionMessage = string.Format(ExceptionMessageTemplate.SameFieldValueAlreadyExist, nameof(Row), nameof(rowUpdate.Number));
                    logger.LogError(exceptionMessage);
                    throw new CustomException(exceptionMessage);
                }
            }
            var rowFromDB = await rowDataAccess.GetRowAsync(rowUpdate.Id);
            if (rowFromDB == null)
            {
                var exceptionMessage = string.Format(ExceptionMessageTemplate.NotFound, nameof(Row), rowUpdate.Id);
                logger.LogError(exceptionMessage);
                throw new NotFoundException(exceptionMessage);
            }
            var soldPlacesInRow = new List<Place>();
            if (rowFromDB.Places != null && rowFromDB.Places.Count > 0)
            {
                var placesInRow = await placeDataAccess.GetPlaceListAsync();
                placesInRow = placesInRow.Where(x => x.RowId == rowFromDB.Id).ToList();
                for (int i = 0; i < placesInRow.Count; i++)
                {
                    var soldTickets = new List<Ticket>();
                    soldTickets = placesInRow[i].Tickets.Where(x => x.IsSold == true).ToList();
                    if (soldTickets.Count > 0)
                    {
                        soldPlacesInRow.Add(placesInRow[i]);
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
                await placeDataAccess.DeletePlaceListAsync(removePlaces);
            }
            if (createPlaceNumbers != null && createPlaceNumbers.Count > 0)
            {
                var createPlaces = new List<Place>();
                for (int i = 0; i < createPlaceNumbers.Count; i++)
                {
                    createPlaces.Add(new Place
                    {
                        Number = createPlaceNumbers[i],
                        Capacity = 1,
                        CreatedOn = DateTime.UtcNow,
                        ModifiedOn = DateTime.UtcNow,
                        RowId = rowFromDB.Id,
                    });
                }
                await placeDataAccess.CreateAsync(createPlaces);
            }
            rowFromDB.Number = rowUpdate.Number;
            rowFromDB.ModifiedOn = DateTime.UtcNow;
            rowFromDB.HallId = rowUpdate.HallId;
            await rowDataAccess.UpdateRowAsync(rowFromDB);
        }
        public async Task<RowDetails> GetAsync(int id)
        {
            var rowFromDB = await rowDataAccess.GetRowAsync(id);
            if (rowFromDB == null)
            {
                var exceptionMessage = string.Format(ExceptionMessageTemplate.NotFound, nameof(Row), id);
                logger.LogError(exceptionMessage);
                throw new NotFoundException(exceptionMessage);
            }
            return new RowDetails
            {
                Id = rowFromDB.Id,
                Number = rowFromDB.Number,
                CreatedOn = rowFromDB.CreatedOn,
                ModifiedOn = rowFromDB.ModifiedOn,
                HallId = rowFromDB.HallId,
                Places = rowFromDB.Places.Select(x => new PlaceDetails
                {
                    Id = x.Id,
                    Capacity = x.Capacity,
                    Number = x.Number,
                    CreatedOn = x.CreatedOn,
                    ModifiedOn = x.ModifiedOn,
                    RowId = x.RowId,
                }).ToList()
            };
        }
        public async Task<List<RowListElement>> GetListAsync()
        {
            var rowsFromDB = await rowDataAccess.GetRowListAsync();
            if (rowsFromDB == null)
            {
                var exceptionMessage = string.Format(ExceptionMessageTemplate.ListNotFound, nameof(Row));
                logger.LogError(exceptionMessage);
                throw new NotFoundException(exceptionMessage);
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
            if (rowFromDB == null)
            {
                var exceptionMessage = string.Format(ExceptionMessageTemplate.NotFound, nameof(Row), id);
                logger.LogError(exceptionMessage);
                throw new NotFoundException(exceptionMessage);
            }
            var soldPlacesInRow = new List<Place>();
            if (rowFromDB.Places != null && rowFromDB.Places.Count > 0)
            {
                var placesInRow = await placeDataAccess.GetPlaceListAsync();
                placesInRow = placesInRow.Where(x => x.RowId == rowFromDB.Id).ToList();
                for (int i = 0; i < placesInRow.Count; i++)
                {
                    var soldTickets = new List<Ticket>();
                    soldTickets = placesInRow[i].Tickets.Where(x => x.IsSold == true).ToList();
                    if (soldTickets.Count > 0)
                    {
                        var exceptionMessage = string.Format(ExceptionMessageTemplate.EntityHasSoldTickets, nameof(Row));
                        logger.LogError(exceptionMessage);
                        throw new CustomException(exceptionMessage);
                    }
                }
            }
            await rowDataAccess.DeleteRowAsync(rowFromDB);
        }
    }
}
