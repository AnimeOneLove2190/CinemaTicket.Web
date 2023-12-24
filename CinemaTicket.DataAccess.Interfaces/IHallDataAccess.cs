using System.Collections.Generic;
using System.Threading.Tasks;
using CinemaTicket.Entities;

namespace CinemaTicket.DataAccess.Interfaces
{
    public interface IHallDataAccess : IBaseDataAccess
    {
        Task<Hall> GetHallAsync(int id);
        Task<Hall> GetHallAsync(string name);
        Task<List<Hall>> GetHallListAsync();
        Task<List<Hall>> GetHallListAsync(List<int> hallIds);
    }
}
