using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public async Task CreateAccountAsync(AccountCreateRequest accountCreate)
        {
            await accountService.CreateAccountAsync(accountCreate);
        }
    }
}
