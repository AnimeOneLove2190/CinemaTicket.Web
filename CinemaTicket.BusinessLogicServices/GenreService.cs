using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Extensions.Logging;
using CinemaTicket.Entities;
using CinemaTicket.BusinessLogic.Interfaces;
using CinemaTicket.DataAccess.Interfaces;
using CinemaTicket.DataTransferObjects.Genres;
using CinemaTicket.Infrastructure.Constants;
using CinemaTicket.Infrastructure.Exceptions;

namespace CinemaTicket.BusinessLogicServices
{
    public class GenreService : IGenreService
    {
        private readonly IGenreDataAccess genreDataAccess;
        private readonly ILogger<GenreService> logger;
        public GenreService(IGenreDataAccess genreDataAccess, ILogger<GenreService> logger)
        {
            this.genreDataAccess = genreDataAccess;
            this.logger = logger;
        }
        public async Task CreateAsync(GenreCreate genreCreate)
        {
            if (genreCreate == null)
            {
                var exceptionMessage = string.Format(ExceptionMessageTemplate.RequestIsNull, nameof(GenreCreate));
                logger.LogError(exceptionMessage);
                throw new CustomException(exceptionMessage);
            }
            if (string.IsNullOrEmpty(genreCreate.Name) || string.IsNullOrWhiteSpace(genreCreate.Name))
            {
                var exceptionMessage = string.Format(ExceptionMessageTemplate.FieldIsRequired, nameof(genreCreate.Name));
                logger.LogError(exceptionMessage);
                throw new CustomException(exceptionMessage);
            }
            var genreFromDB = genreDataAccess.GetGenreAsync(genreCreate.Name);
            if (genreFromDB != null)
            {
                var exceptionMessage = string.Format(ExceptionMessageTemplate.SameNameAlreadyExist, nameof(Genre), genreCreate.Name);
                logger.LogError(exceptionMessage);
                throw new CustomException(exceptionMessage);
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
                var exceptionMessage = string.Format(ExceptionMessageTemplate.CannotBeNullOrNegatevie, nameof(GenreUpdate), nameof(genreUpdate.Id));
                logger.LogError(exceptionMessage);
                throw new CustomException(exceptionMessage);
            }
            if (string.IsNullOrEmpty(genreUpdate.Name) || string.IsNullOrWhiteSpace(genreUpdate.Name))
            {
                var exceptionMessage = string.Format(ExceptionMessageTemplate.FieldIsRequired, nameof(genreUpdate.Name));
                logger.LogError(exceptionMessage);
                throw new CustomException(exceptionMessage);
            }
            var genreFromDB = await genreDataAccess.GetGenreAsync(genreUpdate.Id);
            if (genreFromDB == null)
            {
                var exceptionMessage = string.Format(ExceptionMessageTemplate.NotFound, nameof(Genre), genreUpdate.Id);
                logger.LogError(exceptionMessage);
                throw new NotFoundException(exceptionMessage);
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
                var exceptionMessage = string.Format(ExceptionMessageTemplate.NotFound, nameof(Genre), id);
                logger.LogError(exceptionMessage);
                throw new NotFoundException(exceptionMessage);
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
            if (genresFromDB == null)
            {
                var exceptionMessage = string.Format(ExceptionMessageTemplate.ListNotFound, nameof(Genre));
                logger.LogError(exceptionMessage);
                throw new NotFoundException(exceptionMessage);
            }
            return genresFromDB.Select(x => new GenreListElement
            {
                Id = x.Id,
                Name = x.Name
            }).ToList();
        }
        public async Task DeleteAsync(int id)
        {
            var genreFromDB = await genreDataAccess.GetGenreAsync(id);
            if (genreFromDB == null)
            {
                var exceptionMessage = string.Format(ExceptionMessageTemplate.NotFound, nameof(Genre), id);
                logger.LogError(exceptionMessage);
                throw new NotFoundException(exceptionMessage);
            }
            await genreDataAccess.DeleteGenreAsync(genreFromDB);
        }
    }
}
