using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
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
        Task<List<TicketView>> GetTicketViewList(int sessionId, bool? isSold);
        Task SellTickets(List<int> ticketsIds);
        Task DeleteTickets(List<int> ticketsIds);
        public byte[] GetBulkTicketCreateTemplate();
        Task BulkTicketCreate(IFormFile file, int sessionId);
    }
}
