using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using CinemaTicket.Entities;
using CinemaTicket.DataAccess.Interfaces;
using CinemaTicket.Infrastructure.Helpers;
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
        public async Task<List<Movie>> GetMovieListAsync(string name)
        {
            return await cinemaManagerContext.Movies.Include(x => x.Genres).Where(x => name.ToLower().Contains(x.Name.ToLower())).AsNoTracking().ToListAsync();
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
            cinemaManagerContext.Entry(movie).State = EntityState.Modified;
            await cinemaManagerContext.SaveChangesAsync();
        }
        public async Task DeleteMovieAsync(Movie movie)
        {
            cinemaManagerContext.Entry(movie).State = EntityState.Deleted;
            cinemaManagerContext.Remove(movie);
            await cinemaManagerContext.SaveChangesAsync();
        }
        public async Task<Page<Movie>> GetPageAsync(int page, int pageSize, string movieName)
        {
            var items = await cinemaManagerContext.Movies
                .Include(x => x.Genres)
                .OrderBy(x => x.CreatedOn)
                .Skip(page * pageSize)
                .Take(pageSize)
                .Where(x => movieName.Contains(x.Name))
                .ToListAsync();
            var total = await cinemaManagerContext.Movies.Where(x => movieName.Contains(x.Name)).CountAsync();
            return new Page<Movie>
            {
                Items = items,
                PageNumber = page,
                PageSize = pageSize,
                Total = total,
            };
        }
    }
}
