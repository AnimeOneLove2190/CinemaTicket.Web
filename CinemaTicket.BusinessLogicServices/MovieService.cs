using System;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Http;
using OfficeOpenXml;
using CinemaTicket.Entities;
using CinemaTicket.BusinessLogic.Interfaces;
using CinemaTicket.DataTransferObjects.Movies;
using CinemaTicket.DataTransferObjects.Genres;
using CinemaTicket.DataAccess.Interfaces;
using CinemaTicket.Infrastructure.Constants;
using CinemaTicket.Infrastructure.Exceptions;
using CinemaTicket.Infrastructure.Helpers;
using CinemaTicket.Infrastructure.Settings;

namespace CinemaTicket.BusinessLogicServices
{
    public class MovieService : IMovieService
    {
        private readonly IMovieDataAccess movieDataAccess;
        private readonly IGenreDataAccess genreDataAccess;
        private readonly IAccountService accountService;
        private readonly FileServiceSettings fileServiceSettings;
        private readonly ILogger<MovieService> logger;
        public MovieService(IMovieDataAccess movieDataAccess, 
            IGenreDataAccess genreDataAccess, 
            ILogger<MovieService> logger, 
            IAccountService accountService, 
            IOptions<FileServiceSettings> fileServiceSettings)
        {
            this.movieDataAccess = movieDataAccess;
            this.genreDataAccess = genreDataAccess;
            this.accountService = accountService;
            this.logger = logger;
            this.fileServiceSettings = fileServiceSettings.Value;
        }
        public async Task CreateAsync(MovieCreate movieCreate) 
        {
            var currentUser = await accountService.GetAccountAsync();
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
            if (movieCreate.GenreNames.Count > 0)
            {
                foreach (var genreName in movieCreate.GenreNames)
                {
                    if (string.IsNullOrEmpty(genreName) || string.IsNullOrWhiteSpace(genreName))
                    {
                        movieCreate.GenreNames.Remove(genreName);
                    }
                }
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
                CreatedBy = currentUser.Id,
                ModifiedBy = currentUser.Id,
                Genres = new List<Genre>()
            };
            await movieDataAccess.CreateAsync(movie);
            var genresLower = movieCreate.GenreNames.Select(x => x.ToLower()).ToList();
            var genresFromDB = await genreDataAccess.GetGenreListAsync(genresLower);
            var newGenres = new List<Genre>();
            foreach (var genre in movieCreate.GenreNames)
            {
                var genreExist = genresFromDB.FirstOrDefault(x => x.Name.ToLower() == genre.ToLower());
                if (genreExist != null)
                {
                    movie.Genres.Add(genreExist);
                }
                else
                {
                    newGenres.Add(new Genre
                    {
                        Name = genre,
                        CreatedOn = DateTime.Now,
                        ModifiedOn = DateTime.Now,
                        CreatedBy = currentUser.Id,
                        ModifiedBy = currentUser.Id,
                        Movies = new List<Movie> { movie }
                    });
                }
            }
            await genreDataAccess.CreateListAsync(newGenres);
            await movieDataAccess.CommitAsync();
        }
        public async Task UpdateAsync(MovieUpdate movieUpdate)
        {
            var currentUser = await accountService.GetAccountAsync();
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
                var exceptionMessage = string.Format(ExceptionMessageTemplate.NotFound, nameof(Movie), movieUpdate.Id);
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
            movieFromDB.ModifiedBy = currentUser.Id;
            movieDataAccess.Update(movieFromDB);
            await movieDataAccess.CommitAsync();
        }
        private async Task SetMovieGenreAsync(List<int> genreIds, Movie movie)
        {
            var currentUser = await accountService.GetAccountAsync();

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
                genre.ModifiedBy = currentUser.Id;
            }
            foreach (var genre in newGenres)
            {
                movie.Genres.Add(genre);
                genre.ModifiedOn = DateTime.UtcNow;
                genre.ModifiedBy = currentUser.Id;
            }
        }
        public async Task<MovieDetails> GetAsync(int id)
        {
            var movieFromDB = await movieDataAccess.GetMovieAsync(id);
            if (movieFromDB == null)
            {
                var exceptionMessage = string.Format(ExceptionMessageTemplate.NotFound, nameof(Movie), id);
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
        public async Task SetPosterAsync(IFormFile posterFile, int movieId)
        {
            var currentUser = await accountService.GetAccountAsync();
            var movieFromDB = await movieDataAccess.GetMovieAsync(movieId);
            if (movieFromDB == null)
            {
                var exceptionMessage = string.Format(ExceptionMessageTemplate.NotFound, nameof(Movie), movieId);
                logger.LogError(exceptionMessage);
                throw new NotFoundException(exceptionMessage);
            }
            string posterFileName = null;
            if (posterFile != null && posterFile.Length != 0)
            {
                posterFileName = Guid.NewGuid().ToString() + Path.GetExtension(posterFile.FileName);
                if (!Directory.Exists(fileServiceSettings.ImageFolder))
                {
                    Directory.CreateDirectory(fileServiceSettings.ImageFolder);
                }
                var emblemFilePath = Path.Combine(fileServiceSettings.ImageFolder, posterFileName);
                using (var stream = new FileStream(emblemFilePath, FileMode.Create))
                {
                    posterFile.CopyTo(stream);
                }
            }
            if(!string.IsNullOrEmpty(movieFromDB.PosterFileName))
            {
                string oldPosterFilePath = Path.Combine(fileServiceSettings.ImageFolder, movieFromDB.PosterFileName);
                if (File.Exists(oldPosterFilePath))
                {
                    File.Delete(oldPosterFilePath);
                }
            }
            movieFromDB.PosterFileName = posterFileName;
            movieFromDB.ModifiedOn = DateTime.UtcNow;
            movieFromDB.ModifiedBy = currentUser.Id;
            movieDataAccess.Update(movieFromDB);
            await movieDataAccess.CommitAsync();
        }
        public async Task<PosterView> GetPosterAsync(int movieId)
        {
            var movieFromDB = await movieDataAccess.GetMovieAsync(movieId);
            if (movieFromDB == null)
            {
                var exceptionMessage = string.Format(ExceptionMessageTemplate.NotFound, nameof(Movie), movieId);
                logger.LogError(exceptionMessage);
                throw new NotFoundException(exceptionMessage);
            }
            if (string.IsNullOrEmpty(movieFromDB.PosterFileName))
            {
                var exceptionMessage = string.Format(ExceptionMessageTemplate.NotFound, nameof(Movie.PosterFileName), movieId);
                logger.LogError(exceptionMessage);
                throw new NotFoundException(exceptionMessage);
            }
            string posterFileName = Path.Combine(fileServiceSettings.ImageFolder, movieFromDB.PosterFileName);
            if (!File.Exists(posterFileName))
            {
                var exceptionMessage = string.Format(ExceptionMessageTemplate.NotFound, nameof(File), movieFromDB.PosterFileName);
                logger.LogError(exceptionMessage);
                throw new NotFoundException(exceptionMessage);
            }
            var type = movieFromDB.PosterFileName.Split(".").LastOrDefault().ToLower();
            return new PosterView
            {
                fileArray = File.ReadAllBytes(posterFileName),
                Name = movieFromDB.PosterFileName,
                Type = string.Format(ContentTypeTwo.ImageTemplate, type)
            };
        }
    }
}
