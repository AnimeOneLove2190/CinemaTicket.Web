using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CinemaTicket.Entities;
using CinemaTicket.DataTransferObjects.Sessions;

namespace CinemaTicket.BusinessLogic.Interfaces
{
    public interface ISessionService
    {
        Task CreateAsync(SessionCreate sessionCreate);
        Task UpdateAsync(SessionUpdate sessionUpdate);
        Task<SessionDetails> GetAsync(int id);
        Task<List<SessionListElement>> GetListAsync();
        Task DeleteAsync(int id);
        Task<List<SeansView>> GetSeansViewList(Nullable<DateTime> start, Nullable<DateTime> end);
    }
}
