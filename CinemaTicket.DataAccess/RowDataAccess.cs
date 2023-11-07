using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using CinemaTicket.DataAccess.Interfaces;
using CinemaTicket.Entities;

namespace CinemaTicket.DataAccess
{
    public class RowDataAccess : IRowDataAccess
    {
        private readonly CinemaManagerContext cinemaManagerContext;
        public RowDataAccess(CinemaManagerContext cinemaManagerContext)
        {
            this.cinemaManagerContext = cinemaManagerContext;
        }
        public async Task CreateAsync(Row row)
        {
            await cinemaManagerContext.Rows.AddAsync(row);
            await cinemaManagerContext.SaveChangesAsync();
        }
        public async Task CreateAsync(List<Row> rows)
        {
            await cinemaManagerContext.Rows.AddRangeAsync(rows);
            await cinemaManagerContext.SaveChangesAsync();
        }
        public async Task<Row> GetRowAsync(int id)
        {
            return await cinemaManagerContext.Rows.Include(x => x.Places).FirstOrDefaultAsync(x => x.Id == id);
        }
        public async Task<List<Row>> GetRowListAsync()
        {
            return await cinemaManagerContext.Rows.Include(x => x.Places).AsNoTracking().ToListAsync();
        }
        public async Task<List<Row>> GetRowListAsync(List<int> rowIds)
        {
            return await cinemaManagerContext.Rows.Include(x => x.Places).Where(x => rowIds.Contains(x.Id)).ToListAsync();
        }
        public async Task UpdateRowAsync(Row row)
        {
            row.ModifiedOn = DateTime.Now;
            cinemaManagerContext.Entry(row).State = EntityState.Modified;
            await cinemaManagerContext.SaveChangesAsync();
        }
        public async Task DeleteRowAsync(Row row)
        {
            cinemaManagerContext.Entry(row).State = EntityState.Deleted;
            cinemaManagerContext.Remove(row);
            await cinemaManagerContext.SaveChangesAsync();
        }
    }
}
