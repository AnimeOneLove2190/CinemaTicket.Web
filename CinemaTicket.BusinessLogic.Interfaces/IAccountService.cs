using System.Collections.Generic;
using System.Threading.Tasks;
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
