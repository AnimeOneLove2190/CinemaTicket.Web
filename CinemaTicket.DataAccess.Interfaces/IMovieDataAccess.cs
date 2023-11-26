using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CinemaTicket.Entities;
using CinemaTicket.Infrastructure.Helpers;
using CinemaTicket.DataTransferObjects.Movies;

namespace CinemaTicket.DataAccess.Interfaces
{
    public interface IMovieDataAccess
    {
        Task CreateAsync(Movie movie);
        Task<Movie> GetMovieAsync(int id);
        Task<List<Movie>> GetMovieListAsync(string name);
        Task<List<Movie>> GetMovieListAsync();
        Task<List<Movie>> GetMovieListAsync(List<int> movieIds);
        Task UpdateMovieAsync(Movie movie);
        Task DeleteMovieAsync(Movie movie);
        Task<Page<Movie>> GetPageAsync(MovieSearchRequest movieSearchRequest);
    }
}
