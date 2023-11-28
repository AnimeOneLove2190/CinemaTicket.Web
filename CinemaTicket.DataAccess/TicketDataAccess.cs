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
    public class TicketDataAccess : BaseDataAccess, ITicketDataAccess
    {
        public TicketDataAccess(CinemaManagerContext cinemaManagerContext) : base(cinemaManagerContext)
        {

        }
        public async Task<Ticket> GetTicketAsync(int id)
        {
            return await cinemaManagerContext.Tickets.FirstOrDefaultAsync(x => x.Id == id);
        }
        public async Task<List<Ticket>> GetTicketListAsync()
        {
            return await cinemaManagerContext.Tickets.AsNoTracking().ToListAsync();
        }
        public async Task<List<Ticket>> GetTicketListAsync(List<int> ticketsIds)
        {
            return await cinemaManagerContext.Tickets.Where(x => ticketsIds.Contains(x.Id)).AsNoTracking().ToListAsync();
        }
    }
}
