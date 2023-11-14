using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CinemaTicket.Entities;

namespace CinemaTicket.DataAccess.Interfaces
{
    public interface ITicketDataAccess
    {
        Task CreateAsync(Ticket ticket);
        Task CreateAsync(List<Ticket> tickets);
        Task<Ticket> GetTicketAsync(int id);
        Task<List<Ticket>> GetTicketListAsync();
        Task<List<Ticket>> GetTicketListAsync(List<int> ticketsIds);
        Task UpdateTicketAsync(Ticket ticket);
        Task DeleteTicketAsync(Ticket ticket);
        Task DeleteTicketListAsync(List<Ticket> tickets);
    }
}
