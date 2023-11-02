using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using CinemaTicket.Entities;
using CinemaTicket.BusinessLogic.Interfaces;
using CinemaTicket.DataTransferObjects.Genres;

namespace CinemaTicket.BusinessLogicServices
{
    public class GenreService
    {
        private readonly IGenreDataAccess genreDataAccess;
        public GenreService(IGenreDataAccess genreDataAccess)
        {
            this.genreDataAccess = genreDataAccess;
        }
        public async Task CreateAsync(GenreCreate genreCreate) // Эта мразь не может увидеть GenreCreate, который находится в проекте DTO, поэтому пришлось засунуть его копию в проект с сервисами
        {
            if (genreCreate == null)
            {
                throw new Exception("CreateAsync: One or more input parameters contain null");
            }
            if (string.IsNullOrEmpty(genreCreate.Name) || string.IsNullOrWhiteSpace(genreCreate.Name))
            {
                throw new Exception("CreateAsync: Genre Name field is required");
            }
            var genreFromDB = genreDataAccess.GetGenreAsync(genreCreate.Name);
            if (genreFromDB != null)
            {
                throw new Exception("Genre with the same name already exists");
            }
            var genre = new Genre
            {
                Name = genreCreate.Name,
                Description = genreCreate.Description,
                CreatedOn = DateTime.Now,
                ModifiedOn = DateTime.Now
            };
            await genreDataAccess.CreateAsync(genre);
        }
    }
}
