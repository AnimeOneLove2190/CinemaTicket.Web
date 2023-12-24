using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using CinemaTicket.DataAccess.Interfaces;
using CinemaTicket.Entities;

namespace CinemaTicket.DataAccess
{
    public class HallDataAccess : BaseDataAccess, IHallDataAccess
    {
        public HallDataAccess(CinemaManagerContext cinemaManagerContext) : base(cinemaManagerContext)
        {

        }
        public async Task<Hall> GetHallAsync(int id)
        {
            return await cinemaManagerContext.Halls.Include(x => x.Sessions).Include(x => x.Rows).ThenInclude(x => x.Places).FirstOrDefaultAsync(x => x.Id == id);
        }
        public async Task<Hall> GetHallAsync(string name)
        {
            return await cinemaManagerContext.Halls.Include(x => x.Rows).Include(x => x.Sessions).FirstOrDefaultAsync(x => x.Name.ToLower() == name.ToLower());
        }
        public async Task<List<Hall>> GetHallListAsync()
        {
            return await cinemaManagerContext.Halls.Include(x => x.Rows).Include(x => x.Sessions).AsNoTracking().ToListAsync();
        }
        public async Task<List<Hall>> GetHallListAsync(List<int> hallIds)
        {
            return await cinemaManagerContext.Halls.Include(x => x.Rows).Include(x => x.Sessions).Where(x => hallIds.Contains(x.Id)).ToListAsync();
        }
    }
}
