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
    public class MovieDataAccess : IMovieDataAccess
    {
        private readonly CinemaManagerContext cinemaManagerContext;
        public MovieDataAccess(CinemaManagerContext cinemaManagerContext)
        {
            this.cinemaManagerContext = cinemaManagerContext;
        }
        public async Task CreateAsync(Movie movie)
        {
            await cinemaManagerContext.Movies.AddAsync(movie);
            await cinemaManagerContext.SaveChangesAsync();
        }
        public async Task<Movie> GetMovieAsync(int id)
        {
            return await cinemaManagerContext.Movies.Include(x => x.Genres).FirstOrDefaultAsync(x => x.Id == id);
        }
        public async Task<Movie> GetMovieAsync(string name)
        {
            return await cinemaManagerContext.Movies.Include(x => x.Genres).FirstOrDefaultAsync(x => x.Name.ToLower() == name.ToLower());
        }
        public async Task<List<Movie>> GetMovieListAsync()
        {
            return await cinemaManagerContext.Movies.Include(x => x.Genres).AsNoTracking().ToListAsync();
        }
        public async Task UpdateMovieAsync(Movie movie)
        {
            movie.ModifiedOn = DateTime.Now;
            cinemaManagerContext.Entry(movie).State = EntityState.Modified; // Что это за команда?
            await cinemaManagerContext.SaveChangesAsync();
        }
        public async Task DeleteMovieAsync(Movie movie)
        {
            cinemaManagerContext.Entry(movie).State = EntityState.Deleted; // Что это за команда?
            cinemaManagerContext.Remove(movie);
            await cinemaManagerContext.SaveChangesAsync();
        }
    }
}
