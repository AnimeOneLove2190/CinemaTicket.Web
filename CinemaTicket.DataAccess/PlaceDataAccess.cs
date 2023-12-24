using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using CinemaTicket.Entities;
using CinemaTicket.DataAccess.Interfaces;

namespace CinemaTicket.DataAccess
{
    public class PlaceDataAccess : BaseDataAccess, IPlaceDataAccess
    {
        public PlaceDataAccess(CinemaManagerContext cinemaManagerContext) : base(cinemaManagerContext)
        {

        }
        public async Task<Place> GetPlaceAsync(int id)
        {
            return await cinemaManagerContext.Places.Include(x => x.Tickets).FirstOrDefaultAsync(x => x.Id == id);
        }
        public async Task<List<Place>> GetPlaceListAsync()
        {
            return await cinemaManagerContext.Places.Include(x => x.Tickets).AsNoTracking().ToListAsync();
        }
        public async Task<List<Place>> GetPlaceListAsync(List<int> placesIds)
        {
            return await cinemaManagerContext.Places.Include(x => x.Tickets).Where(x => placesIds.Contains(x.Id)).ToListAsync();
        }
    }
}
