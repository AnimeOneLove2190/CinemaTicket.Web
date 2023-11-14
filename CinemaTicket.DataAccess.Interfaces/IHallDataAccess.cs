using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CinemaTicket.Entities;

namespace CinemaTicket.DataAccess.Interfaces
{
    public interface IHallDataAccess
    {
        Task CreateAsync(Hall hall);
        Task CreateAsync(List<Hall> halls);
        Task<Hall> GetHallAsync(int id);
        Task<Hall> GetHallAsync(string name);
        Task<List<Hall>> GetHallListAsync();
        Task<List<Hall>> GetHallListAsync(List<int> hallIds);
        Task UpdateHallAsync(Hall hall);
        Task DeleteHallAsync(Hall hall);
    }
}
