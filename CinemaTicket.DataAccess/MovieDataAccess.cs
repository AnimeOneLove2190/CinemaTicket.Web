using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using CinemaTicket.Entities;
using CinemaTicket.DataAccess.Interfaces;
using CinemaTicket.Infrastructure.Helpers;
using CinemaTicket.DataTransferObjects.Movies;
namespace CinemaTicket.DataAccess
{
    public class MovieDataAccess : BaseDataAccess, IMovieDataAccess
    {
        public MovieDataAccess(CinemaManagerContext cinemaManagerContext) : base(cinemaManagerContext)
        {

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
        public async Task<List<Movie>> GetMovieListAsync(List<int> movieIds)
        {
            return await cinemaManagerContext.Movies.Include(x => x.Genres).Where(x => movieIds.Contains(x.Id)).ToListAsync();
        }
        public async Task<Page<Movie>> GetPageAsync(MovieSearchRequest movieSearchRequest)
        {
            var query = cinemaManagerContext.Movies.Include(x => x.Genres).AsQueryable();
            if (!string.IsNullOrEmpty(movieSearchRequest.MovieName) || !string.IsNullOrWhiteSpace(movieSearchRequest.MovieName))
            {
                query = query.Where(x => x.Name.ToLower().Contains(movieSearchRequest.MovieName.ToLower()));
            }
            if (!string.IsNullOrEmpty(movieSearchRequest.Description) || !string.IsNullOrWhiteSpace(movieSearchRequest.Description))
            {
                query = query.Where(x => x.Description.ToLower().Contains(movieSearchRequest.Description.ToLower()));
            }
            if (movieSearchRequest.MinDuration.HasValue)
            {
                query = query.Where(x => x.Duration >= movieSearchRequest.MinDuration);
            }
            if (movieSearchRequest.MaxDuration.HasValue)
            {
                query = query.Where(x => x.Duration <= movieSearchRequest.MaxDuration);
            }
            if (movieSearchRequest.GenreIds.Count > 0)
            {
                query = query.Where(x => x.Genres.Any(g => movieSearchRequest.GenreIds.Contains(g.Id)));
            }
            var items = await query
                .OrderBy(x => x.CreatedOn)
                .Skip(movieSearchRequest.PageNumber * movieSearchRequest.PageSize)
                .Take(movieSearchRequest.PageSize)
                .ToListAsync();
            var total = await query.CountAsync();
            return new Page<Movie>
            {
                Items = items,
                PageNumber = movieSearchRequest.PageNumber,
                PageSize = movieSearchRequest.PageSize,
                Total = total,
            };
        }
    }
}
