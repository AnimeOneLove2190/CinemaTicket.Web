using System;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using CinemaTicket.Entities;
using CinemaTicket.BusinessLogic.Interfaces;
using CinemaTicket.DataTransferObjects.Movies;

namespace CinemaTicket.BusinessLogicServices
{
    class MovieService
    {
        private readonly IMovieDataAccess movieDataAccess;
        private readonly IGenreDataAccess genreDataAccess;
        public MovieService(IMovieDataAccess movieDataAccess, IGenreDataAccess genreDataAccess)
        {
            this.movieDataAccess = movieDataAccess;
            this.genreDataAccess = genreDataAccess;
        }
        public async Task CreateAsync(string movieName, string movieDescription, int movieDuration, List<string> genreNames) 
        {
            if (string.IsNullOrEmpty(movieName) || string.IsNullOrWhiteSpace(movieName))
            {
                throw new Exception("CreateAsync: Movie Name field is required");
            }
            if (string.IsNullOrEmpty(movieDescription) || string.IsNullOrWhiteSpace(movieDescription))
            {
                throw new Exception("CreateAsync: Movie Description field is required");
            }
            if (movieDuration <= 0)
            {
                throw new Exception("CreateMovie: Movie Duration field must not be empty or contain a negative value");
            }
            var movieFromDB = movieDataAccess.GetMovieAsync(movieName);
            if (movieFromDB != null)
            {
                if (movieFromDB.Result.Name.ToLower() == movieName.ToLower() && movieFromDB.Result.Description.ToLower() == movieDescription.ToLower())
                {
                    throw new Exception("A Movie with the same name and the same description already exists");
                }
            }
            var movie = new Movie
            {
                Name = movieName,
                Description = movieDescription,
                Duration = movieDuration,
                CreatedOn = DateTime.Now,
                ModifiedOn = DateTime.Now
            };
            await movieDataAccess.CreateAsync(movie);
            var genresFromDB = genreDataAccess.GetGenreListAsync().Result.Select(x => x.Name.ToLower()).ToList();
            var needAddGenres = new List<string>();
            for (int i = 0; i < genreNames.Count; i++)
            {
                if(genresFromDB.Contains(genreNames[i].ToLower()))
                {
                    continue;
                }
                needAddGenres.Add(genreNames[i]);
            }
            if (needAddGenres != null && needAddGenres.Count > 0)
            {
                var genres = needAddGenres.Select(x => new Genre
                {
                    Name = x,
                    CreatedOn = DateTime.Now,
                    ModifiedOn = DateTime.Now
                }).ToList();
                await genreDataAccess.CreateAsync(genres);
            }
        }
    }
}
