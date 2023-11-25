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
    public class SessionDataAccess : ISessionDataAccess
    {
        private readonly CinemaManagerContext cinemaManagerContext;
        public SessionDataAccess(CinemaManagerContext cinemaManagerContext)
        {
            this.cinemaManagerContext = cinemaManagerContext;
        }
        public async Task CreateAsync(Session session)
        {
            await cinemaManagerContext.Sessions.AddAsync(session);
            await cinemaManagerContext.SaveChangesAsync();
        }
        public async Task CreateAsync(List<Session> sessions)
        {
            await cinemaManagerContext.Sessions.AddRangeAsync(sessions);
            await cinemaManagerContext.SaveChangesAsync();
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
        public async Task UpdateSessionAsync(Session session)
        {
            session.ModifiedOn = DateTime.Now;
            cinemaManagerContext.Entry(session).State = EntityState.Modified;
            await cinemaManagerContext.SaveChangesAsync();
        }
        public async Task DeleteSessionAsync(Session session)
        {
            cinemaManagerContext.Entry(session).State = EntityState.Deleted;
            cinemaManagerContext.Remove(session);
            await cinemaManagerContext.SaveChangesAsync();
        }
        public async Task DeleteSessionListAsync(List<Session> sessions)
        {
            foreach (var session in sessions)
            {
                cinemaManagerContext.Entry(session).State = EntityState.Deleted;
            }
            cinemaManagerContext.RemoveRange(sessions);
            await cinemaManagerContext.SaveChangesAsync();
        }
        public async Task<List<Session>> GetSessionListInPeriodAsync (DateTime startDate, DateTime endDate)
        {
            return await cinemaManagerContext.Sessions.Include(x => x.Tickets).Include(x => x.Hall).Include(x => x.Movie).Where(x => x.Start >= startDate && x.Start <= endDate).AsNoTracking().ToListAsync();
        }
    }
}
