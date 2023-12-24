using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using CinemaTicket.Entities;
using CinemaTicket.DataAccess.Interfaces;

namespace CinemaTicket.DataAccess
{
    public class GenreDataAccess : BaseDataAccess, IGenreDataAccess
    {
        public GenreDataAccess(CinemaManagerContext cinemaManagerContext) : base(cinemaManagerContext)
        {

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
        public async Task<List<Genre>> GetGenreListAsync(List<int> genreIds)
        {
            return await cinemaManagerContext.Genres.Include(x => x.Movies).Where(x => genreIds.Contains(x.Id)).ToListAsync();
        }
        public async Task<List<Genre>> GetGenreListAsync(List<string> genreNames)
        {
            return await cinemaManagerContext.Genres.Include(x => x.Movies).Where(x => genreNames.Contains(x.Name.ToLower())).ToListAsync();
        }
    }
}

