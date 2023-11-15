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
    }
}
