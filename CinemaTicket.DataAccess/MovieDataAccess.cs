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
        public async Task<Movie> GetMovieAsync(string name) //хуйня, переделать
        {
            return await cinemaManagerContext.Movies.Include(x => x.Genres).FirstOrDefaultAsync(x => x.Name.ToLower() == name.ToLower());
        }
        public async Task<List<Movie>> GetMovieListAsync()
        {
            return await cinemaManagerContext.Movies.Include(x => x.Genres).AsNoTracking().ToListAsync();
        }
        public async Task<List<Movie>> GetMovieListAsync(List<int> movieIds) // Вдруг пригодится
        {
            return await cinemaManagerContext.Movies.Include(x => x.Genres).Where(x => movieIds.Contains(x.Id)).ToListAsync();
        }
        public async Task UpdateMovieAsync(Movie movie)
        {
            movie.ModifiedOn = DateTime.Now;
            cinemaManagerContext.Entry(movie).State = EntityState.Modified; // Помогает решить проблему, если сущность получена как AsNoTracking, аналогично для Deleted
            await cinemaManagerContext.SaveChangesAsync();
        }
        public async Task DeleteMovieAsync(Movie movie)
        {
            cinemaManagerContext.Entry(movie).State = EntityState.Deleted; // Помогает решить проблему, если сущность получена как AsNoTracking, аналогично для Deleted
            cinemaManagerContext.Remove(movie);
            await cinemaManagerContext.SaveChangesAsync();
        }
    }
}
