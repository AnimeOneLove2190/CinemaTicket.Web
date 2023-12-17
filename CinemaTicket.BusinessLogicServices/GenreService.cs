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
        private readonly IValidationService validationService;
        private readonly ILogger<GenreService> logger;
        public GenreService(IGenreDataAccess genreDataAccess, ILogger<GenreService> logger, IAccountService accountService, IValidationService validationService)
        {
            this.genreDataAccess = genreDataAccess;
            this.accountService = accountService;
            this.validationService = validationService;
            this.logger = logger;
        }
        public async Task CreateAsync(GenreCreate genreCreate)
        {
            var currentUser = await accountService.GetAccountAsync();
            validationService.ValidationRequestIsNull(genreCreate);
            validationService.ValidationFieldIsRequiered(nameof(genreCreate.Name), genreCreate.Name);
            var genreFromDB = await genreDataAccess.GetGenreAsync(genreCreate.Name);
            validationService.ValidationNameAlreadyExist(genreFromDB, genreCreate.Name);
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
            validationService.ValidationCannotBeNullOrNegative(genreUpdate, nameof(genreUpdate.Id), genreUpdate.Id);
            validationService.ValidationFieldIsRequiered(nameof(genreUpdate.Name), genreUpdate.Name);
            var genreFromDB = await genreDataAccess.GetGenreAsync(genreUpdate.Id);
            validationService.ValidationNotFound(genreUpdate, genreUpdate.Id);
            var genreWithSameName = await genreDataAccess.GetGenreAsync(genreUpdate.Name.ToLower());
            if (genreWithSameName != null && genreWithSameName.Id != genreFromDB.Id)
            {
                validationService.ValidationNameAlreadyExist(genreWithSameName, genreUpdate.Name);
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
            validationService.ValidationNotFound(genreFromDB, id);
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
            validationService.ValidationNotFound(genreFromDB, id);
            genreDataAccess.Delete(genreFromDB);
            await genreDataAccess.CommitAsync();
        }
    }
}
