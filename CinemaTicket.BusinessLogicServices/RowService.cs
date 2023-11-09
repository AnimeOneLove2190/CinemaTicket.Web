using System;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using CinemaTicket.Entities;
using CinemaTicket.BusinessLogic.Interfaces;
using CinemaTicket.DataTransferObjects.Rows;
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
    }
}
