using System;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using CinemaTicket.Entities;
using CinemaTicket.BusinessLogic.Interfaces;
using CinemaTicket.DataTransferObjects.Movies;
using CinemaTicket.DataTransferObjects.Genres;
using CinemaTicket.DataAccess.Interfaces;
using CinemaTicket.Infrastructure.Constants;
using CinemaTicket.Infrastructure.Exceptions;
using CinemaTicket.Infrastructure.Helpers;

namespace CinemaTicket.BusinessLogicServices
{
    public class MovieService : IMovieService
    {
        private readonly IMovieDataAccess movieDataAccess;
        private readonly IGenreDataAccess genreDataAccess;
        private readonly ILogger<MovieService> logger;
        public MovieService(IMovieDataAccess movieDataAccess, IGenreDataAccess genreDataAccess, ILogger<MovieService> logger)
        {
            this.movieDataAccess = movieDataAccess;
            this.genreDataAccess = genreDataAccess;
            this.logger = logger;
        }
        public async Task CreateAsync(MovieCreate movieCreate) 
        {
            if (string.IsNullOrEmpty(movieCreate.Name) || string.IsNullOrWhiteSpace(movieCreate.Name))
            {
                var exceptionMessage = string.Format(ExceptionMessageTemplate.FieldIsRequired, nameof(movieCreate.Name));
                logger.LogError(exceptionMessage);
                throw new CustomException(exceptionMessage);
            }
            if (string.IsNullOrEmpty(movieCreate.Description) || string.IsNullOrWhiteSpace(movieCreate.Description))
            {
                var exceptionMessage = string.Format(ExceptionMessageTemplate.FieldIsRequired, nameof(movieCreate.Name));
                logger.LogError(exceptionMessage);
                throw new CustomException(exceptionMessage);
            }
            if (movieCreate.Duration <= 0)
            {
                var exceptionMessage = string.Format(ExceptionMessageTemplate.CannotBeNullOrNegatevie, nameof(MovieCreate), nameof(movieCreate.Duration));
                logger.LogError(exceptionMessage);
                throw new CustomException(exceptionMessage);
            }
            var movieListFromDB = await movieDataAccess.GetMovieListAsync(movieCreate.Name);
            foreach (var movieFromDB in movieListFromDB)
            {
                if (movieFromDB != null)
                {
                    if (movieFromDB.Name.ToLower() == movieCreate.Name.ToLower() && movieFromDB.Description.ToLower() == movieCreate.Description.ToLower())
                    {
                        var exceptionMessage = string.Format(ExceptionMessageTemplate.SameNameAlreadyExist, nameof(Movie), movieCreate.Name);
                        logger.LogError(exceptionMessage);
                        throw new CustomException(exceptionMessage);
                    }
                }
            }
            var movie = new Movie
            {
                Name = movieCreate.Name,
                Description = movieCreate.Description,
                Duration = movieCreate.Duration,
                CreatedOn = DateTime.Now,
                ModifiedOn = DateTime.Now,
                Genres = new List<Genre>()
            };
            await movieDataAccess.CreateAsync(movie);
            var genresFromDB = await genreDataAccess.GetGenreListAsync();
            var genreNamesFromDB = genresFromDB.Select(x => x.Name).ToList();
            var needAddGenres = new List<string>();
            for (int i = 0; i < movieCreate.GenreNames.Count; i++)
            {
                if(genreNamesFromDB.Contains(movieCreate.GenreNames[i].ToLower()))
                {
                    movie.Genres.Add(genresFromDB[i]);
                    continue;
                }
                needAddGenres.Add(movieCreate.GenreNames[i]);
            }
            if (needAddGenres != null && needAddGenres.Count > 0)
            {
                var genres = needAddGenres.Select(x => new Genre
                {
                    Name = x,
                    CreatedOn = DateTime.Now,
                    ModifiedOn = DateTime.Now,
                    Movies = new List<Movie> { movie }
                }).ToList();
                await genreDataAccess.CreateListAsync(genres);
            }
            await movieDataAccess.CommitAsync();
        }
        public async Task UpdateAsync(MovieUpdate movieUpdate)
        {
            if (string.IsNullOrEmpty(movieUpdate.Name) || string.IsNullOrWhiteSpace(movieUpdate.Name))
            {
                var exceptionMessage = string.Format(ExceptionMessageTemplate.FieldIsRequired, nameof(movieUpdate.Name));
                logger.LogError(exceptionMessage);
                throw new CustomException(exceptionMessage);
            }
            if (string.IsNullOrEmpty(movieUpdate.Description) || string.IsNullOrWhiteSpace(movieUpdate.Description))
            {
                var exceptionMessage = string.Format(ExceptionMessageTemplate.FieldIsRequired, nameof(movieUpdate.Description));
                logger.LogError(exceptionMessage);
                throw new CustomException(exceptionMessage);
            }
            if (movieUpdate.Duration <= 0)
            {
                var exceptionMessage = string.Format(ExceptionMessageTemplate.CannotBeNullOrNegatevie, nameof(MovieUpdate), nameof(movieUpdate.Duration));
                logger.LogError(exceptionMessage);
                throw new CustomException(exceptionMessage);
            }
            var movieFromDB = await movieDataAccess.GetMovieAsync(movieUpdate.Id);
            if (movieFromDB == null)
            {
                var exceptionMessage = string.Format(ExceptionMessageTemplate.NotFound, movieUpdate.Id);
                logger.LogError(exceptionMessage);
                throw new NotFoundException(exceptionMessage);
            }
            var uniqGenreIds = movieUpdate.GenreIds.Distinct().ToList();
            if (uniqGenreIds.Count != movieUpdate.GenreIds.Count)
            {
                var exceptionMessage = string.Format(ExceptionMessageTemplate.Duplicate, nameof(movieUpdate.GenreIds));
                logger.LogError(exceptionMessage);
                throw new CustomException(exceptionMessage);
            }
            if (movieUpdate.GenreIds == null)
            {
                movieUpdate.GenreIds = new List<int>();
            }
            await SetMovieGenreAsync(movieUpdate.GenreIds, movieFromDB);
            movieFromDB.Name = movieUpdate.Name;
            movieFromDB.Description = movieUpdate.Description;
            movieFromDB.Duration = movieUpdate.Duration;
            movieFromDB.ModifiedOn = DateTime.Now;
            movieDataAccess.Update(movieFromDB);
            await movieDataAccess.CommitAsync();
        }
        private async Task SetMovieGenreAsync(List<int> genreIds, Movie movie)
        {
            var genres = await genreDataAccess.GetGenreListAsync(genreIds);
            if (genres.Count != genreIds.Count)
            {
                var exceptionMessage = string.Format(ExceptionMessageTemplate.NotAllFound, nameof(genreIds));
                logger.LogError(exceptionMessage);
                throw new NotFoundException(exceptionMessage);
            }
            var oldGenreIds = movie.Genres.Select(x => x.Id).ToList();
            var newGenreIds = genreIds.Except(oldGenreIds).ToList();
            var removeGenreIds = oldGenreIds.Except(genreIds).ToList();
            var removeGenres = movie.Genres.Where(x => removeGenreIds.Contains(x.Id)).ToList();
            var newGenres = genres.Where(x => newGenreIds.Contains(x.Id)).ToList();
            foreach (var genre in removeGenres)
            {
                movie.Genres.Remove(genre);
                genre.ModifiedOn = DateTime.Now;
            }
            foreach (var genre in newGenres)
            {
                movie.Genres.Add(genre);
                genre.ModifiedOn = DateTime.UtcNow;
            }
        }
        public async Task<MovieDetails> GetAsync(int id)
        {
            var movieFromDB = await movieDataAccess.GetMovieAsync(id);
            if (movieFromDB == null)
            {
                var exceptionMessage = string.Format(ExceptionMessageTemplate.NotFound, id);
                logger.LogError(exceptionMessage);
                throw new NotFoundException(exceptionMessage);
            }
            return new MovieDetails
            {
                Id = movieFromDB.Id,
                Name = movieFromDB.Name,
                Description = movieFromDB.Description,
                CreatedOn = movieFromDB.CreatedOn,
                ModifiedOn = movieFromDB.ModifiedOn,
                Genres = movieFromDB.Genres.Select(x => new GenreDetails
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    CreatedOn = x.CreatedOn,
                    ModifiedOn = x.ModifiedOn,
                }).ToList()
            };
        }
        public async Task<List<MovieListElement>> GetListAsync()
        {
            var moviesFromDB = await movieDataAccess.GetMovieListAsync();
            if (moviesFromDB == null || moviesFromDB.Count == 0)
            {
                var exceptionMessage = string.Format(ExceptionMessageTemplate.NotFound, nameof(Movie));
                logger.LogError(exceptionMessage);
                throw new NotFoundException(exceptionMessage);
            }
            return moviesFromDB.Select(x => new MovieListElement
            {
                Id = x.Id,
                Name = x.Name,
            }).ToList();
        }
        public async Task<Page<MoviePageView>> GetPageAsync(MovieSearchRequest movieSearch)
        {
            var moviePage = await movieDataAccess.GetPageAsync(movieSearch);
            return new Page<MoviePageView>
            {
                Items = moviePage.Items.Select(x => new MoviePageView
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    Duration = x.Duration,
                    GenreNames = x.Genres.Select(x => x.Name).ToList()
                }).ToList(),
                PageNumber = moviePage.PageNumber,
                PageSize = moviePage.PageSize,
                Total = moviePage.Total,
            };
        }

    }
}
