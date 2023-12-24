using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CinemaTicket.Entities;

namespace CinemaTicket.DataAccess.Interfaces
{
    public interface IAccountDataAccess
    {
        Task CreateAsync(Account account);
        Task<bool> LoginExistAsync(string login);
        Task<bool> EmailExistAsync(string email);
        Task<Account> GetAccountAsync(string login);
        Task<Account> GetAccountAsync(Guid id);
        Task<List<Account>> GetAccountListAsync();
    }
}
