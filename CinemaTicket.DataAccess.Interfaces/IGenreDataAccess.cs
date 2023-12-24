using System.Collections.Generic;
using System.Threading.Tasks;
using CinemaTicket.Entities;


namespace CinemaTicket.DataAccess.Interfaces
{
    public interface IGenreDataAccess : IBaseDataAccess
    {
        Task<Genre> GetGenreAsync(int id);
        Task<Genre> GetGenreAsync(string name);
        Task<List<Genre>> GetGenreListAsync();
        Task<List<Genre>> GetGenreListAsync(List<int> genreIds);
        Task<List<Genre>> GetGenreListAsync(List<string> genreNames);
    }
}
