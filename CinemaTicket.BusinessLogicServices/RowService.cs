using System;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
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
        public RowService(IHallDataAccess hallDataAccess, IRowDataAccess rowDataAccess, IPlaceDataAccess placeDataAccess)
        {
            this.hallDataAccess = hallDataAccess;
            this.rowDataAccess = rowDataAccess;
            this.placeDataAccess = placeDataAccess;
        }
        public async Task CreateAsync(RowCreate rowCreate)
        {
            if (rowCreate == null)
            {
                throw new Exception();
            }
            if (rowCreate.HallId <= 0)
            {
                throw new Exception();
            }
            if (rowCreate.Number <= 0)
            {
                throw new Exception();
            }
            var hallFromDB = await hallDataAccess.GetHallAsync(rowCreate.HallId);
            if (hallFromDB == null)
            {
                throw new Exception();
            }
            var rowsNumbers = hallFromDB.Rows.Select(x => x.Number).ToList();
            if (rowsNumbers != null)
            {
                if (rowsNumbers.Contains(rowCreate.Number))
                {
                    throw new Exception();
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
                throw new Exception();
            }
            if (rowUpdate.Id <= 0)
            {
                throw new Exception();
            }
            if (rowUpdate.HallId <= 0)
            {
                throw new Exception();
            }
            if (rowUpdate.Number <= 0)
            {
                throw new Exception();
            }
            var hallFromDB = await hallDataAccess.GetHallAsync(rowUpdate.HallId);
            if (hallFromDB == null)
            {
                throw new Exception();
            }
            var rowWithTheSameNumber = hallFromDB.Rows.FirstOrDefault(x => x.Number == rowUpdate.Number);
            if (rowWithTheSameNumber != null)
            {
                if (rowWithTheSameNumber.Id != rowUpdate.Id)
                {
                    throw new Exception();
                }
            }
            var rowFromDB = await rowDataAccess.GetRowAsync(rowUpdate.Id);
            if (rowFromDB == null)
            {
                throw new Exception();
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
                throw new Exception();
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
            if (rowsFromDB == null || rowsFromDB.Count == 0)
            {
                throw new Exception();
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
                throw new Exception();
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
                        throw new Exception();
                    }
                }
            }
            await rowDataAccess.DeleteRowAsync(rowFromDB);
        }
    }
}
