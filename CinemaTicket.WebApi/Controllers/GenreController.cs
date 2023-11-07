using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CinemaTicket.BusinessLogic.Interfaces;
using CinemaTicket.DataTransferObjects.Genres;

namespace CinemaTicket.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenreController : ControllerBase
    {
        private readonly IGenreService genreService;
        public GenreController(IGenreService genreService)
        {
            this.genreService = genreService;
        }
        [HttpPost]
        [Route("AddGenre")]
        public async Task CreateMovieAsync(GenreCreate genreCreate)
        {
            await genreService.CreateAsync(genreCreate);
        }
        [HttpPost]
        [Route("UpdateGenre")]
        public async Task UpdateMovieAsync(GenreUpdate genreUpdate)
        {
            await genreService.UpdateAsync(genreUpdate);
        }
        [HttpGet]
        [Route("GetGenre")]
        public async Task<GenreDetails> GetGenreAsync(int id)
        {
            return await genreService.GetAsync(id);
        }
        [HttpGet]
        [Route("GetGenreList")]
        public async Task<List<GenreListElement>> GetGenreListAsync()
        {
            return await genreService.GetListAsync();
        }
    }
}
