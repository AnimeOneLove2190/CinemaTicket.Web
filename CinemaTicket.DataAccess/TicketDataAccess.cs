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
    public class TicketDataAccess : ITicketDataAccess
    {
        private readonly CinemaManagerContext cinemaManagerContext;
        public TicketDataAccess(CinemaManagerContext cinemaManagerContext)
        {
            this.cinemaManagerContext = cinemaManagerContext;
        }
        public async Task CreateAsync(Ticket ticket)
        {
            await cinemaManagerContext.Tickets.AddAsync(ticket);
            await cinemaManagerContext.SaveChangesAsync();
        }
        public async Task CreateAsync(List<Ticket> tickets)
        {
            await cinemaManagerContext.Tickets.AddRangeAsync(tickets);
            await cinemaManagerContext.SaveChangesAsync();
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
        public async Task UpdateTicketAsync(Ticket ticket)
        {
            ticket.ModifiedOn = DateTime.Now;
            cinemaManagerContext.Entry(ticket).State = EntityState.Modified;
            await cinemaManagerContext.SaveChangesAsync();
        }
        public async Task UpdateTicketListAsync(List<Ticket> tickets) //TODO Проверить сваггером
        {
            foreach (var ticket in tickets)
            {
                ticket.ModifiedOn = DateTime.UtcNow;
                cinemaManagerContext.Entry(ticket).State = EntityState.Modified;
            }
            cinemaManagerContext.UpdateRange(tickets);
            await cinemaManagerContext.SaveChangesAsync();
        }
        public async Task DeleteTicketAsync(Ticket ticket)
        {
            cinemaManagerContext.Entry(ticket).State = EntityState.Deleted;
            cinemaManagerContext.Remove(ticket);
            await cinemaManagerContext.SaveChangesAsync();
        }
        public async Task DeleteTicketListAsync(List<Ticket> tickets)
        {
            foreach (var ticket in tickets)
            {
                cinemaManagerContext.Entry(ticket).State = EntityState.Deleted;
            }
            cinemaManagerContext.RemoveRange(tickets);
            await cinemaManagerContext.SaveChangesAsync();
        }
    }
}
