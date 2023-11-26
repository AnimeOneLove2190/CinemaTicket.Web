using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using CinemaTicket.BusinessLogic.Interfaces;
using CinemaTicket.DataAccess.Interfaces;
using CinemaTicket.DataTransferObjects.Accounts;
using CinemaTicket.Entities;
using CinemaTicket.Infrastructure.Constants;
using CinemaTicket.Infrastructure.Exceptions;

namespace CinemaTicket.BusinessLogicServices
{
    public class AccountService : IAccountService
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IAccountDataAccess accountDataAccess;
        private readonly ILogger<AccountService> logger;
        public AccountService(IHttpContextAccessor httpContextAccessor, IAccountDataAccess accountDataAccess, ILogger<AccountService> logger)
        {
            this.httpContextAccessor = httpContextAccessor;
            this.accountDataAccess = accountDataAccess;
            this.logger = logger;
        }
        public async Task CreateAccountAsync(AccountCreateRequest accountCreateRequest)
        {
            if (await accountDataAccess.LoginExistAsync(accountCreateRequest.Login))
            {
                var exceptionMessage = string.Format(ExceptionMessageTemplate.SameNameAlreadyExist, nameof(Account.Login), accountCreateRequest.Login);
                logger.LogError(exceptionMessage);
                throw new CustomException(exceptionMessage);
            }
            if (await accountDataAccess.EmailExistAsync(accountCreateRequest.Email))
            {
                var exceptionMessage = string.Format(ExceptionMessageTemplate.SameNameAlreadyExist, nameof(Account.Email), accountCreateRequest.Email);
                logger.LogError(exceptionMessage);
                throw new CustomException(exceptionMessage);
            }
            var account = new Account()
            {
                CreatedOn = DateTime.Now,
                Email = accountCreateRequest.Email.ToLower(),
                HashPassword = GetHash(accountCreateRequest.Password),
                Id = Guid.NewGuid(),
                Login = accountCreateRequest.Login.ToLower(),
                RoleId = Permissions.Id.DefaultUser
            };
            await accountDataAccess.CreateAsync(account);
        }
        private string GetHash(string input)
        {
            var md5 = MD5.Create();
            var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(input));

            return Convert.ToBase64String(hash);
        }
    }
}
