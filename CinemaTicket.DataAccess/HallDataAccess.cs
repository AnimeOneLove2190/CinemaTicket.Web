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
    public class HallDataAccess : IHallDataAccess
    {
        private readonly CinemaManagerContext cinemaManagerContext;
        public HallDataAccess(CinemaManagerContext cinemaManagerContext)
        {
            this.cinemaManagerContext = cinemaManagerContext;
        }
        public async Task CreateAsync(Hall hall)
        {
            await cinemaManagerContext.Halls.AddAsync(hall);
            await cinemaManagerContext.SaveChangesAsync();
        }
        public async Task CreateAsync(List<Hall> halls)
        {
            await cinemaManagerContext.Halls.AddRangeAsync(halls);
            await cinemaManagerContext.SaveChangesAsync();
        }
        public async Task<Hall> GetHallAsync(int id)
        {
            return await cinemaManagerContext.Halls.Include(x => x.Rows).Include(x => x.Sessions).FirstOrDefaultAsync(x => x.Id == id); // Пока неизвестно, работают ли два инклюда одновременно, сделать гет и проверить
        }
        public async Task<Hall> GetHallAsync(string name)
        {
            return await cinemaManagerContext.Halls.Include(x => x.Rows).Include(x => x.Sessions).FirstOrDefaultAsync(x => x.Name.ToLower() == name.ToLower()); // Тоже неизвестно
        }
        public async Task<List<Hall>> GetHallListAsync()
        {
            return await cinemaManagerContext.Halls.Include(x => x.Rows).Include(x => x.Sessions).AsNoTracking().ToListAsync(); // Та же хуйня
        }
        public async Task<List<Hall>> GetHallListAsync(List<int> hallIds)
        {
            return await cinemaManagerContext.Halls.Include(x => x.Rows).Include(x => x.Sessions).Where(x => hallIds.Contains(x.Id)).ToListAsync(); // И тут ещё 
        }
        public async Task UpdateHallAsync(Hall hall)
        {
            hall.ModifiedOn = DateTime.Now;
            cinemaManagerContext.Entry(hall).State = EntityState.Modified;
            await cinemaManagerContext.SaveChangesAsync();
        }
        public async Task DeleteHallAsync(Hall hall)
        {
            cinemaManagerContext.Entry(hall).State = EntityState.Deleted;
            cinemaManagerContext.Remove(hall);
            await cinemaManagerContext.SaveChangesAsync();
        }
    }
}
