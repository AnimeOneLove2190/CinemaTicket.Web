using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CinemaTicket.DataTransferObjects.Sessions;
using CinemaTicket.DataTransferObjects.Tickets;

namespace CinemaTicket.BusinessLogic.Interfaces
{
    public interface IStatisticService
    {
        Task<List<SeansView>> GetSeansViewList(Nullable<DateTime> start, Nullable<DateTime> end);
        Task<List<TicketView>> GetTicketViewList(int sessionId, Nullable<bool> isSold);
        Task DeleteTickets(List<int> ticketsIds);
        Task SellTickets(List<int> ticketsIds);
    }
}
