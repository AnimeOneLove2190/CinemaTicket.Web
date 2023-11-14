using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using CinemaTicket.Entities;
using CinemaTicket.DataAccess.Interfaces;

namespace CinemaTicket.DataAccess
{
    public class PlaceDataAccess : IPlaceDataAccess
    {
        private readonly CinemaManagerContext cinemaManagerContext;
        public PlaceDataAccess(CinemaManagerContext cinemaManagerContext)
        {
            this.cinemaManagerContext = cinemaManagerContext;
        }
        public async Task CreateAsync(Place place)
        {
            await cinemaManagerContext.Places.AddAsync(place);
            await cinemaManagerContext.SaveChangesAsync();
        }
        public async Task CreateAsync(List<Place> places)
        {
            await cinemaManagerContext.Places.AddRangeAsync(places);
            await cinemaManagerContext.SaveChangesAsync();
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
        public async Task UpdatePlaceAsync(Place place)
        {
            place.ModifiedOn = DateTime.Now;
            cinemaManagerContext.Entry(place).State = EntityState.Modified;
            await cinemaManagerContext.SaveChangesAsync();
        }
        public async Task DeletePlaceAsync(Place place)
        {
            cinemaManagerContext.Entry(place).State = EntityState.Deleted;
            cinemaManagerContext.Remove(place);
            await cinemaManagerContext.SaveChangesAsync();
        }
        public async Task DeletePlaceListAsync(List<Place> places)
        {
            foreach (var place in places)
            {
                cinemaManagerContext.Entry(place).State = EntityState.Deleted;
            }
            cinemaManagerContext.RemoveRange(places);
            await cinemaManagerContext.SaveChangesAsync();
        }
    }
}
