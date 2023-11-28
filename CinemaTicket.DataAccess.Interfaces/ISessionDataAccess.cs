using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CinemaTicket.Entities;

namespace CinemaTicket.DataAccess.Interfaces
{
    public interface ISessionDataAccess : IBaseDataAccess
    {
        Task<Session> GetSessionAsync(int id);
        Task<Session> GetSessionAsync(DateTime start, int hallId);
        Task<List<Session>> GetSessionListAsync();
        Task<List<Session>> GetSessionListAsync(List<int> sessionsIds);
        Task<List<Session>> GetSessionListInPeriodAsync(DateTime startDate, DateTime endDate);
    }
}
