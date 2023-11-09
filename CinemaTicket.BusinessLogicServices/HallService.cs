using System;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using CinemaTicket.Entities;
using CinemaTicket.BusinessLogic.Interfaces;
using CinemaTicket.DataTransferObjects.Halls;
using CinemaTicket.DataAccess.Interfaces;

namespace CinemaTicket.BusinessLogicServices
{
    public class HallService : IHallService
    {
        private readonly IHallDataAccess hallDataAccess;
        private readonly IRowDataAccess rowDataAccess;
        private readonly IPlaceDataAccess placeDataAccess;
        public HallService(IHallDataAccess hallDataAccess, IRowDataAccess rowDataAccess, IPlaceDataAccess placeDataAccess)
        {
            this.hallDataAccess = hallDataAccess;
            this.rowDataAccess = rowDataAccess;
            this.placeDataAccess = placeDataAccess;
        }
        public async Task CreateAsync(HallCreate hallCreate)
        {
            if (hallCreate == null)
            {
                throw new Exception();
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
                throw new Exception();
            }
            if (hallUpdate.Id <= 0)
            {
                throw new Exception();
            }
            var hallFromDB = await hallDataAccess.GetHallAsync(hallUpdate.Id);
            if (hallFromDB == null)
            {
                throw new Exception();
            }
            if (hallFromDB.Rows != null || hallFromDB.Rows.Count > 0)
            {
                var rowsIds = hallFromDB.Rows.Select(x => x.Id).ToList();
                var placesInHall = placeDataAccess.GetPlaceListAsync().Result.Where(x => rowsIds.Contains(x.RowId)).ToList();
                for (int i = 0; i < placesInHall.Count; i++)
                {
                    var soldTickets = placesInHall[i].Tickets.Where(x => x.IsSold == true).ToList();
                    if (soldTickets.Count > 0)
                    {
                        throw new Exception();
                    }
                }
            }
            hallUpdate.RowsNumbers = hallUpdate.RowsNumbers.Distinct().ToList();
            if (hallUpdate.RowsNumbers == null)
            {
                hallUpdate.RowsNumbers = new List<int>();
            }
            var rowsNumbersFromDB = hallFromDB.Rows.Select(x => x.Number).ToList();
            var removeRowsNumbers = rowsNumbersFromDB.Except(hallUpdate.RowsNumbers).ToList();
            var createRowsNumbers = hallUpdate.RowsNumbers.Except(rowsNumbersFromDB).ToList();
            if (removeRowsNumbers != null && removeRowsNumbers.Count > 0)
            {
                var removeRows = hallFromDB.Rows.Where(x => removeRowsNumbers.Contains(x.Number)).ToList();
                await rowDataAccess.DeleteRowListAsync(removeRows);
            }
            if (createRowsNumbers != null && removeRowsNumbers.Count > 0)
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
    }
}
