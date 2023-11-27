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
        public async Task<AccountView> GetAccountAsync()
        {
            var user = httpContextAccessor.HttpContext.User.Identity.Name;
            var entity = await accountDataAccess.GetAccountAsync(user);
            if(entity == null)
            {
                var exceptionMessage = string.Format(ExceptionMessageTemplate.NotFound, nameof(Account), user);
                logger.LogError(exceptionMessage);
                throw new NotFoundException(exceptionMessage);
            }
            return new AccountView()
            {
                Id = entity.Id,
                Login = entity.Login,
                Email = entity.Email,
                RoleId = entity.RoleId,
                RoleName = entity.Role.Name,
            };
        }
        public async Task<List<AccountView>> GetAccountListAsync()
        {
            var entities = await accountDataAccess.GetAccountListAsync();
            return entities.Select(x => new AccountView
            {
                Id = x.Id,
                Login = x.Login,
                Email = x.Email,
                RoleId = x.RoleId,
                RoleName = x.Role.Name,
            }).ToList();
        }
        public async Task LoginAsync(string login, string password)
        {
            var account = await accountDataAccess.GetAccountAsync(login);
            if (account != null && account.HashPassword == GetHash(password.ToLower()))
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimsIdentity.DefaultNameClaimType, login),
                    new Claim(ClaimsIdentity.DefaultRoleClaimType, account.Role.Name)
                };
                ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
                await httpContextAccessor.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
            }
            else
            {
                logger.LogError(ExceptionMessageTemplate.WrongLoginOrPassword);
                throw new CustomException(ExceptionMessageTemplate.WrongLoginOrPassword);
            }
        }
        public async Task LogoutAsync()
        {
            await httpContextAccessor.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }
        private string GetHash(string input)
        {
            var md5 = MD5.Create();
            var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(input));

            return Convert.ToBase64String(hash);
        }
    }
}
