using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using CinemaTicket.DataAccess.Interfaces;
using CinemaTicket.Entities;

namespace CinemaTicket.DataAccess
{
    public class RowDataAccess : BaseDataAccess, IRowDataAccess
    {
        public RowDataAccess(CinemaManagerContext cinemaManagerContext) : base(cinemaManagerContext)
        {

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
    }
}
