using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CinemaTicket.Entities;
using CinemaTicket.DataTransferObjects.Tickets;

namespace CinemaTicket.BusinessLogic.Interfaces
{
    public interface ITicketService
    {
        Task CreateAsync(TicketCreate ticketCreate);
        Task UpdateAsync(TicketUpdate ticketUpdate);
        Task<TicketDetails> GetAsync(int id);
        Task<List<TicketListElement>> GetListAsync();
        Task DeleteAsync(int id);
    }
}
