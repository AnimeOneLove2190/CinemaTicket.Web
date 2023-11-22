using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CinemaTicket.Entities;
using CinemaTicket.DataTransferObjects.Sessions;
using CinemaTicket.DataTransferObjects.Statistic;

namespace CinemaTicket.BusinessLogic.Interfaces
{
    public interface IStatisticService
    {
        Task<List<Statictic>> GetStaticticList(Nullable<DateTime> start, Nullable<DateTime> end);
        Task<List<TicketStatistic>> GetTicketStaticticList(int sessionId, Nullable<bool> isSold);
    }
}
