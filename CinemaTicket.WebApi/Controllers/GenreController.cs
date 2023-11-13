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
        public async Task CreateGenreAsync(GenreCreate genreCreate)
        {
            await genreService.CreateAsync(genreCreate);
        }
        [HttpPost]
        [Route("UpdateGenre")]
        public async Task UpdateGenreAsync(GenreUpdate genreUpdate)
        {
            await genreService.UpdateAsync(genreUpdate);
        }
        [HttpPost]
        [Route("DeleteGenre")]
        public async Task DeleteGenreAsync(int id)
        {
            await genreService.DeleteAsync(id);
        }
    }
}
