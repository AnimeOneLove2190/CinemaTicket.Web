using System;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using CinemaTicket.Entities;
using CinemaTicket.BusinessLogic.Interfaces;
using CinemaTicket.DataTransferObjects.Places;
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
    }
}
