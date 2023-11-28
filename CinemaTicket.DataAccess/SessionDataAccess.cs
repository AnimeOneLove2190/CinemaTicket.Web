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
    public class SessionDataAccess : BaseDataAccess, ISessionDataAccess
    {
        public SessionDataAccess(CinemaManagerContext cinemaManagerContext) : base(cinemaManagerContext)
        {

        }
        public async Task<Session> GetSessionAsync(int id)
        {
            return await cinemaManagerContext.Sessions.Include(x => x.Tickets).ThenInclude(x => x.Place).FirstOrDefaultAsync(x => x.Id == id);
        }
        public async Task<Session> GetSessionAsync(DateTime start, int hallId)
        {
            return await cinemaManagerContext.Sessions.Include(x => x.Tickets).FirstOrDefaultAsync(x => x.Start == start && x.HallId == hallId);
        }
        public async Task<List<Session>> GetSessionListAsync()
        {
            return await cinemaManagerContext.Sessions.Include(x => x.Tickets).AsNoTracking().ToListAsync();
        }
        public async Task<List<Session>> GetSessionListAsync(List<int> sessionsIds)
        {
            return await cinemaManagerContext.Sessions.Include(x => x.Tickets).Where(x => sessionsIds.Contains(x.Id)).ToListAsync();
        }
        public async Task<List<Session>> GetSessionListInPeriodAsync (DateTime startDate, DateTime endDate)
        {
            return await cinemaManagerContext.Sessions.Include(x => x.Tickets).Include(x => x.Hall).Include(x => x.Movie).Where(x => x.Start >= startDate && x.Start <= endDate).AsNoTracking().ToListAsync();
        }
    }
}
