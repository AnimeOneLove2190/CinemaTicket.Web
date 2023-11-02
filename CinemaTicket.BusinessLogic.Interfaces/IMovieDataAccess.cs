using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CinemaTicket.Entities;
using CinemaTicket.DataTransferObjects.Genres;

namespace CinemaTicket.BusinessLogic.Interfaces
{
    public interface IMovieDataAccess
    {
        Task CreateAsync(Movie movie);
        Task<Movie> GetMovieAsync(int id);
        Task<Movie> GetMovieAsync(string name);
        Task<List<Movie>> GetMovieListAsync();
        Task UpdateMovieAsync(Movie movie);
        Task DeleteMovieAsync(Movie movie);
    }
}
