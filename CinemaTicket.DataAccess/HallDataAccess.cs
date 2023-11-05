using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using CinemaTicket.Entities;

namespace CinemaTicket.DataAccess
{
    class HallDataAccess
    {
        private readonly CinemaManagerContext cinemaManagerContext;
        public HallDataAccess(CinemaManagerContext cinemaManagerContext)
        {
            this.cinemaManagerContext = cinemaManagerContext;
        }
        //public async Task CreateAsync(Hall hall)
        //{
        //    await cinemaManagerContext.AddAsync(hall);
        //    await cinemaManagerContext.SaveChangesAsync();
        //}
        //public async Task<Hall> GetHallAsync(int id)
        //{
        //    return await cinemaManagerContext.Halls.FirstOrDefaultAsync(x => x.Id == id);
        //}
    }
}
