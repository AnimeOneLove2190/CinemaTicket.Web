using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using System.Threading.Tasks;
using CinemaTicket.DataTransferObjects.Accounts;
using CinemaTicket.BusinessLogic.Interfaces;

namespace CinemaTicket.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService accountService;
        public AccountController(IAccountService accountService)
        {
            this.accountService = accountService;
        }

        [HttpPost]
        [Route("AddAccount")]
        [AllowAnonymous]
        public async Task CreateAccountAsync([FromBody] AccountCreateRequest accountCreate)
        {
            await accountService.CreateAccountAsync(accountCreate);
        }

        [HttpGet]
        [Route("Get")]
        [Authorize]
        public async Task<AccountView> GetAccount()
        {
            return await accountService.GetAccountAsync();
        }

        [HttpGet]
        [Route("GetListAccount")]
        [Authorize(Roles = "Admin")]
        public async Task<List<AccountView>> GetListAccount()
        {
            return await accountService.GetAccountListAsync();
        }

        [HttpPost]
        [Route("Login")]
        [AllowAnonymous]
        public async Task Login(string login, string password)
        {
            await accountService.LoginAsync(login, password);
        }

        [HttpPost]
        [Route("Logout")]
        [Authorize]
        public async Task Logout()
        {
            await accountService.LogoutAsync();
        }
    }
}
