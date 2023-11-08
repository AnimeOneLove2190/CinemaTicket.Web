using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using CinemaTicket.Entities;
using CinemaTicket.BusinessLogic.Interfaces;
using CinemaTicket.DataAccess.Interfaces;
using CinemaTicket.DataTransferObjects.Genres;

namespace CinemaTicket.BusinessLogicServices
{
    public class GenreService : IGenreService
    {
        private readonly IGenreDataAccess genreDataAccess;
        public GenreService(IGenreDataAccess genreDataAccess)
        {
            this.genreDataAccess = genreDataAccess;
        }
        public async Task CreateAsync(GenreCreate genreCreate)
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
        public async Task UpdateAsync(GenreUpdate genreUpdate)
        {
            if (genreUpdate.Id <= 0)
            {
                throw new Exception();
            }
            if (string.IsNullOrEmpty(genreUpdate.Name) || string.IsNullOrWhiteSpace(genreUpdate.Name))
            {
                throw new Exception();
            }
            var genreFromDB = await genreDataAccess.GetGenreAsync(genreUpdate.Id);
            if (genreFromDB == null)
            {
                throw new Exception();
            }
            genreFromDB.Name = genreUpdate.Name;
            genreFromDB.Description = genreUpdate.Description;
            genreFromDB.ModifiedOn = DateTime.Now;
            await genreDataAccess.UpdateGenreAsync(genreFromDB);
        }
        public async Task<GenreDetails> GetAsync(int id)
        {
            var genreFromDB = await genreDataAccess.GetGenreAsync(id);
            if (genreFromDB == null)
            {
                throw new Exception();
            }
            return new GenreDetails
            {
                Id = genreFromDB.Id,
                Name = genreFromDB.Name,
                Description = genreFromDB.Description,
                CreatedOn = genreFromDB.CreatedOn,
                ModifiedOn = genreFromDB.ModifiedOn,
            };
        }
        public async Task<List<GenreListElement>> GetListAsync()
        {
            var genresFromDB = await genreDataAccess.GetGenreListAsync();
            if (genresFromDB == null || genresFromDB.Count == 0)
            {
                throw new Exception();
            }
            return genresFromDB.Select(x => new GenreListElement
            {
                Id = x.Id,
                Name = x.Name
            }).ToList();
        }
    }
}
