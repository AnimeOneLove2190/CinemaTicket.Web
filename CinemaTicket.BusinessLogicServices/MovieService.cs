using System;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using CinemaTicket.Entities;
using CinemaTicket.BusinessLogic.Interfaces;
using CinemaTicket.DataTransferObjects.Movies;
using CinemaTicket.DataAccess.Interfaces;

namespace CinemaTicket.BusinessLogicServices
{
    class MovieService : IMovieService
    {
        private readonly IMovieDataAccess movieDataAccess;
        private readonly IGenreDataAccess genreDataAccess;
        public MovieService(IMovieDataAccess movieDataAccess, IGenreDataAccess genreDataAccess)
        {
            this.movieDataAccess = movieDataAccess;
            this.genreDataAccess = genreDataAccess;
        }
        public async Task CreateAsync(MovieCreate movieCreate) 
        {
            if (string.IsNullOrEmpty(movieCreate.Name) || string.IsNullOrWhiteSpace(movieCreate.Name))
            {
                throw new Exception("CreateAsync: Movie Name field is required");
            }
            if (string.IsNullOrEmpty(movieCreate.Description) || string.IsNullOrWhiteSpace(movieCreate.Description))
            {
                throw new Exception("CreateAsync: Movie Description field is required");
            }
            if (movieCreate.Duration <= 0)
            {
                throw new Exception("CreateMovie: Movie Duration field must not be empty or contain a negative value");
            }
            var movieFromDB = await movieDataAccess.GetMovieAsync(movieCreate.Name);
            if (movieFromDB != null)
            {
                if (movieFromDB.Name.ToLower() == movieCreate.Name.ToLower() && movieFromDB.Description.ToLower() == movieCreate.Description.ToLower())
                {
                    throw new Exception("A Movie with the same name and the same description already exists");
                }
            }
            var movie = new Movie
            {
                Name = movieCreate.Name,
                Description = movieCreate.Description,
                Duration = movieCreate.Duration,
                CreatedOn = DateTime.Now,
                ModifiedOn = DateTime.Now,
            };
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
                await movieDataAccess.CreateAsync(movie);
                await genreDataAccess.CreateAsync(genres);
            }
        }
    }
}
