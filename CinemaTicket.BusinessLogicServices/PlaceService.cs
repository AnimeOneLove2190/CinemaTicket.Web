using System;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using CinemaTicket.Entities;
using CinemaTicket.BusinessLogic.Interfaces;
using CinemaTicket.DataTransferObjects.Places;
using CinemaTicket.DataTransferObjects.Tickets;
using CinemaTicket.DataAccess.Interfaces;

namespace CinemaTicket.BusinessLogicServices
{
    public class PlaceService : IPlaceService
    {
        private readonly IRowDataAccess rowDataAccess;
        private readonly IPlaceDataAccess placeDataAccess;
        public PlaceService(IHallDataAccess hallDataAccess, IRowDataAccess rowDataAccess, IPlaceDataAccess placeDataAccess)
        {
            this.rowDataAccess = rowDataAccess;
            this.placeDataAccess = placeDataAccess;
        }
        public async Task CreateAsync(PlaceCreate placeCreate)
        {
            if (placeCreate == null)
            {
                throw new Exception();
            }
            if (placeCreate.RowId <= 0)
            {
                throw new Exception();
            }
            if (placeCreate.Capacity <= 0)
            {
                throw new Exception();
            }
            var rowFromDB = await rowDataAccess.GetRowAsync(placeCreate.RowId);
            if (rowFromDB == null)
            {
                throw new Exception();
            }
            var rowsNumbers = rowFromDB.Places.Select(x => x.Number).ToList();
            if (rowsNumbers != null)
            {
                if (rowsNumbers.Contains(placeCreate.Number))
                {
                    throw new Exception();
                }
            }
            var place = new Place
            {
                Number = placeCreate.Number,
                RowId = placeCreate.RowId,
                Capacity = placeCreate.Capacity,
                CreatedOn = DateTime.UtcNow,
                ModifiedOn = DateTime.UtcNow,
            };
            await placeDataAccess.CreateAsync(place);
        }
        public async Task UpdateAsync(PlaceUpdate placeUpdate)
        {
            if (placeUpdate == null)
            {
                throw new Exception();
            }
            if (placeUpdate.Id <= 0)
            {
                throw new Exception();
            }
            if (placeUpdate.RowId <= 0)
            {
                throw new Exception();
            }
            if (placeUpdate.Number <= 0)
            {
                throw new Exception();
            }
            if (placeUpdate.Capacity <= 0)
            {
                throw new Exception();
            }
            var placeFromDB = await placeDataAccess.GetPlaceAsync(placeUpdate.Id);
            if (placeFromDB == null)
            {
                throw new Exception();
            }
            var rowRFromDB = await rowDataAccess.GetRowAsync(placeUpdate.RowId);
            if (rowRFromDB == null)
            {
                throw new Exception();
            }
            var placeWithTheSameNumber = rowRFromDB.Places.FirstOrDefault(x => x.Number == placeUpdate.Number);
            if (placeWithTheSameNumber != null)
            {
                if (placeWithTheSameNumber.Id != placeUpdate.Id)
                {
                    throw new Exception();
                }
            }
            var soldTickets = placeFromDB.Tickets.Where(x => x.IsSold == true).ToList();
            if (soldTickets.Count > 0)
            {
                throw new Exception();
            }
            placeFromDB.Number = placeUpdate.Number;
            placeFromDB.Capacity = placeUpdate.Capacity;
            placeFromDB.ModifiedOn = DateTime.UtcNow;
            placeFromDB.RowId = placeFromDB.RowId;
            await placeDataAccess.UpdatePlaceAsync(placeFromDB);
        }
        public async Task<PlaceDetails> GetAsync(int id)
        {
            var placeFromDB = await placeDataAccess.GetPlaceAsync(id);
            if (placeFromDB == null)
            {
                throw new Exception();
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
            if (placesFromDB == null || placesFromDB.Count == 0)
            {
                throw new Exception();
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
                throw new Exception();
            }
            var soldTickets = placeFromDB.Tickets.Where(X => X.IsSold == true).ToList();
            if (soldTickets.Count > 0)
            {
                throw new Exception();
            }
            await placeDataAccess.DeletePlaceAsync(placeFromDB);
        }
    }
}
