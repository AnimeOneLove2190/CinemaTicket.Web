using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CinemaTicket.BusinessLogic.Interfaces;
using CinemaTicket.DataTransferObjects.Movies;

namespace CinemaTicket.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MovieController : ControllerBase
    {
        private readonly IMovieService movieService;
        public MovieController(IMovieService movieService)
        {
            this.movieService = movieService;
        }
        [HttpPost]
        [Route("AddMovie")]
        public async Task CreateMovieAsync(string movieName, string movieDescription, int movieDuration, List<string> genreNames)
        {
            var movieCreate = new MovieCreate
            {
                Name = movieName,
                Description = movieDescription,
                Duration = movieDuration,
                GenreNames = genreNames,
            };
            await movieService.CreateAsync(movieCreate);
        }
    }
}
