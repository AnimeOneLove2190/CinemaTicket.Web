using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CinemaTicket.Entities;
using CinemaTicket.DataTransferObjects.Accounts;
namespace CinemaTicket.BusinessLogic.Interfaces
{
    public interface IAccountService
    {
        Task CreateAccountAsync(AccountCreateRequest accountCreateRequest);
        Task<AccountView> GetAccountAsync();
        Task<List<AccountView>> GetAccountListAsync();
        Task LoginAsync(string login, string password);
        Task LogoutAsync();
    }
}
