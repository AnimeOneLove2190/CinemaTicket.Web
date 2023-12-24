using System.Collections.Generic;
using System.Threading.Tasks;
using CinemaTicket.Entities;

namespace CinemaTicket.DataAccess.Interfaces
{
    public interface ITicketDataAccess : IBaseDataAccess
    {
        Task<Ticket> GetTicketAsync(int id);
        Task<List<Ticket>> GetTicketListAsync();
        Task<List<Ticket>> GetTicketListAsync(List<int> ticketsIds);
    }
}
