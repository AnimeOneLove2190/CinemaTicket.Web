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
    class GenreDataAccess : IGenreDataAccess
    {
        private readonly CinemaManagerContext cinemaManagerContext;
        public GenreDataAccess(CinemaManagerContext cinemaManagerContext)
        {
            this.cinemaManagerContext = cinemaManagerContext;
        }
        public async Task CreateAsync(Genre genre)
        {
            await cinemaManagerContext.Genres.AddAsync(genre);
            await cinemaManagerContext.SaveChangesAsync();
        }
        public async Task CreateAsync(List<Genre> genres)
        {
            await cinemaManagerContext.Genres.AddRangeAsync(genres);
            await cinemaManagerContext.SaveChangesAsync();
        }
        public async Task<Genre> GetGenreAsync(int id)
        {
            return await cinemaManagerContext.Genres.Include(x => x.Movies).FirstOrDefaultAsync(x => x.Id == id);
        }
        public async Task<Genre> GetGenreAsync(string name)
        {
            return await cinemaManagerContext.Genres.Include(x => x.Movies).FirstOrDefaultAsync(x => x.Name.ToLower() == name.ToLower());
        }
        public async Task<List<Genre>> GetGenreListAsync()
        {
            return await cinemaManagerContext.Genres.Include(x => x.Movies).AsNoTracking().ToListAsync();
        }
        public async Task UpdateGenreAsync(Genre genre)
        {
            genre.ModifiedOn = DateTime.Now;
            cinemaManagerContext.Entry(genre).State = EntityState.Modified; // Что это за команда?
            await cinemaManagerContext.SaveChangesAsync();
        }
        public async Task DeleteGenreAsync(Genre genre)
        {
            cinemaManagerContext.Entry(genre).State = EntityState.Deleted; // Что это за команда?
            cinemaManagerContext.Remove(genre);
            await cinemaManagerContext.SaveChangesAsync();
        }
    }
}

