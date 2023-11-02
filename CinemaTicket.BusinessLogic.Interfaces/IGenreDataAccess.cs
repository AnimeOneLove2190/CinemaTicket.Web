using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CinemaTicket.Entities;


namespace CinemaTicket.BusinessLogic.Interfaces
{
    public interface IGenreDataAccess
    {
        Task CreateAsync(Genre genre);
        Task CreateAsync(List<Genre> genres);
        Task<Genre> GetGenreAsync(int id);
        Task<Genre> GetGenreAsync(string name);
        Task<List<Genre>> GetGenreListAsync();
        Task UpdateGenreAsync(Genre genre);
        Task DeleteGenreAsync(Genre genre);
    }
}
