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
        private readonly IAccountService accountService;
        private readonly ILogger<GenreService> logger;
        public GenreService(IGenreDataAccess genreDataAccess, ILogger<GenreService> logger, IAccountService accountService)
        {
            this.genreDataAccess = genreDataAccess;
            this.accountService = accountService;
            this.logger = logger;
        }
        public async Task CreateAsync(GenreCreate genreCreate)
        {
            var currentUser = await accountService.GetAccountAsync();
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
            var genreFromDB = await genreDataAccess.GetGenreAsync(genreCreate.Name);
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
                ModifiedOn = DateTime.Now,
                CreatedBy = currentUser.Id,
                ModifiedBy = currentUser.Id
            };
            await genreDataAccess.CreateAsync(genre);
            await genreDataAccess.CommitAsync();
        }
        public async Task UpdateAsync(GenreUpdate genreUpdate)
        {
            var currentUser = await accountService.GetAccountAsync();
            if (genreUpdate.Id <= 0)
            {
                var exceptionMessage = string.Format(ExceptionMessageTemplate.CannotBeNullOrNegative, nameof(GenreUpdate), nameof(genreUpdate.Id));
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
            var genreWithSameName = await genreDataAccess.GetGenreAsync(genreUpdate.Name.ToLower());
            if (genreWithSameName != null)
            {
                if (genreWithSameName.Id != genreFromDB.Id)
                {
                    var exceptionMessage = string.Format(ExceptionMessageTemplate.SameNameAlreadyExist, nameof(Genre), genreUpdate.Name);
                    logger.LogError(exceptionMessage);
                    throw new CustomException(exceptionMessage);
                }
            }
            genreFromDB.Name = genreUpdate.Name;
            genreFromDB.Description = genreUpdate.Description;
            genreFromDB.ModifiedOn = DateTime.Now;
            genreFromDB.ModifiedBy = currentUser.Id;
            genreDataAccess.Update(genreFromDB);
            await genreDataAccess.CommitAsync();
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
                genresFromDB = new List<Genre>();
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
            genreDataAccess.Delete(genreFromDB);
            await genreDataAccess.CommitAsync();
        }
    }
}
