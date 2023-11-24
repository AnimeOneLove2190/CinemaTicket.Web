﻿using System;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using CinemaTicket.Entities;
using CinemaTicket.BusinessLogic.Interfaces;
using CinemaTicket.DataTransferObjects.Halls;
using CinemaTicket.DataTransferObjects.Rows;
using CinemaTicket.DataAccess.Interfaces;
using CinemaTicket.Infrastructure.Constants;
using CinemaTicket.Infrastructure.Exceptions;


namespace CinemaTicket.BusinessLogicServices
{
    public class HallService : IHallService
    {
        private readonly IHallDataAccess hallDataAccess;
        private readonly IRowDataAccess rowDataAccess;
        private readonly IPlaceDataAccess placeDataAccess;
        private readonly ILogger<HallService> logger;
        public HallService(IHallDataAccess hallDataAccess, IRowDataAccess rowDataAccess, IPlaceDataAccess placeDataAccess, ILogger<HallService> logger)
        {
            this.hallDataAccess = hallDataAccess;
            this.rowDataAccess = rowDataAccess;
            this.placeDataAccess = placeDataAccess;
            this.logger = logger;
        }
        public async Task CreateAsync(HallCreate hallCreate)
        {
            if (hallCreate == null)
            {
                var exceptionMessage = string.Format(ExceptionMessageTemplate.RequestIsNull, nameof(HallCreate));
                logger.LogError(exceptionMessage);
                throw new CustomException(exceptionMessage);
            }
            var hall = new Hall
            {
                Name = hallCreate.Name,
                CreatedOn = DateTime.UtcNow,
                ModifiedOn = DateTime.UtcNow,
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
                        HallId = hall.Id
                    });
                }
                await rowDataAccess.CreateAsync(rows);
            }
        }
        public async Task UpdateAsync(HallUpdate hallUpdate)
        {
            if (hallUpdate == null)
            {
                var exceptionMessage = string.Format(ExceptionMessageTemplate.RequestIsNull, nameof(HallUpdate));
                logger.LogError(exceptionMessage);
                throw new CustomException(exceptionMessage);
            }
            if (hallUpdate.Id <= 0)
            {
                var exceptionMessage = string.Format(ExceptionMessageTemplate.CannotBeNullOrNegatevie, nameof(HallUpdate), nameof(hallUpdate.Id));
                logger.LogError(exceptionMessage);
                throw new CustomException(exceptionMessage);
            }
            var hallFromDB = await hallDataAccess.GetHallAsync(hallUpdate.Id);
            if (hallFromDB == null)
            {
                var exceptionMessage = string.Format(ExceptionMessageTemplate.NotFound, nameof(Hall), hallUpdate.Id);
                logger.LogError(exceptionMessage);
                throw new NotFoundException(exceptionMessage);
            }
            var soldPlacesInHall = new List<Place>();
            if (hallFromDB.Rows != null || hallFromDB.Rows.Count > 0)
            {
                var rowsIds = hallFromDB.Rows.Select(x => x.Id).ToList();
                var allPlaces = await placeDataAccess.GetPlaceListAsync();
                var placesInHall = allPlaces.Where(x => rowsIds.Contains(x.RowId)).ToList();
                for (int i = 0; i < placesInHall.Count; i++)
                {
                    var soldTickets = placesInHall[i].Tickets.Where(x => x.IsSold == true).ToList();
                    if (soldTickets.Count > 0)
                    {
                        soldPlacesInHall.Add(placesInHall[i]);
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
                await rowDataAccess.DeleteRowListAsync(removeRows);
            }
            if (createRowsNumbers != null && createRowsNumbers.Count > 0)
            {
                var createRows = new List<Row>();
                for (int i = 0; i < createRowsNumbers.Count; i++)
                {
                    createRows.Add(new Row
                    {
                        Number = createRowsNumbers[i],
                        CreatedOn = DateTime.UtcNow,
                        ModifiedOn = DateTime.UtcNow,
                        HallId = hallFromDB.Id,
                    });
                }
                await rowDataAccess.CreateAsync(createRows);
            }
            hallFromDB.Name = hallUpdate.Name;
            hallFromDB.ModifiedOn = DateTime.UtcNow;
            await hallDataAccess.UpdateHallAsync(hallFromDB);
        }
        public async Task<HallDetails> GetAsync(int id)
        {
            var hallFromDB = await hallDataAccess.GetHallAsync(id);
            if (hallFromDB == null)
            {
                var exceptionMessage = string.Format(ExceptionMessageTemplate.NotFound, nameof(Hall), id);
                logger.LogError(exceptionMessage);
                throw new NotFoundException(exceptionMessage);
            }
            return new HallDetails
            {
                Id = hallFromDB.Id,
                Name = hallFromDB.Name,
                CreatedOn = hallFromDB.CreatedOn,
                ModifiedOn = hallFromDB.ModifiedOn,
                Rows = hallFromDB.Rows.Select(x => new RowDetails
                {
                    Id = x.Id,
                    Number = x.Number,
                    CreatedOn = x.CreatedOn,
                    ModifiedOn = x.ModifiedOn,
                    HallId = x.HallId,
                }).ToList()
            };
        }
        public async Task<List<HallListElement>> GetListAsync()
        {
            var hallsFromDB = await hallDataAccess.GetHallListAsync();
            if (hallsFromDB == null)
            {
                var exceptionMessage = string.Format(ExceptionMessageTemplate.ListNotFound, nameof(Hall));
                logger.LogError(exceptionMessage);
                throw new NotFoundException(exceptionMessage);
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
            if (hallFromDB == null)
            {
                var exceptionMessage = string.Format(ExceptionMessageTemplate.NotFound, nameof(Hall), id);
                logger.LogError(exceptionMessage);
                throw new NotFoundException(exceptionMessage);
            }
            var soldPlacesInHall = new List<Place>();
            if (hallFromDB.Rows != null || hallFromDB.Rows.Count > 0)
            {
                var rowsIds = hallFromDB.Rows.Select(x => x.Id).ToList();
                var allPlaces = await placeDataAccess.GetPlaceListAsync();
                var placesInHall = allPlaces.Where(x => rowsIds.Contains(x.RowId)).ToList();
                for (int i = 0; i < placesInHall.Count; i++)
                {
                    var soldTickets = placesInHall[i].Tickets.Where(x => x.IsSold == true).ToList();
                    if (soldTickets.Count > 0)
                    {
                        var exceptionMessage = string.Format(ExceptionMessageTemplate.EntityHasSoldTickets, nameof(Hall));
                        logger.LogError(exceptionMessage);
                        throw new CustomException(exceptionMessage);
                    }
                }
            }
            await hallDataAccess.DeleteHallAsync(hallFromDB);
        }
    }
}
