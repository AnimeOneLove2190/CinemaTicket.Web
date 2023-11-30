using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CinemaTicket.BusinessLogic.Interfaces;
using CinemaTicket.DataTransferObjects.Movies;
using CinemaTicket.Infrastructure.Helpers;

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
        [Authorize]
        public async Task CreateMovieAsync(MovieCreate movieCreate)
        {
            await movieService.CreateAsync(movieCreate);
        }
        [HttpPost]
        [Route("UpdateMovie")]
        [Authorize]
        public async Task UpdateMovieAsync(MovieUpdate movieUpdate)
        {
            await movieService.UpdateAsync(movieUpdate);
        }
        [HttpGet]
        [Route("GetMovie")]
        public async Task<MovieDetails> GetGenreAsync(int id)
        {
            return await movieService.GetAsync(id);
        }
        [HttpGet]
        [Route("GetMovieList")]
        public async Task<List<MovieListElement>> GetGenreListAsync()
        {
            return await movieService.GetListAsync();
        }
        [HttpPost]
        [Route("GetPage")]
        public async Task<Page<MoviePageView>> GetPageAsync(MovieSearchRequest movieSearch)
        {
            return await movieService.GetPageAsync(movieSearch);
        }
    }
}
