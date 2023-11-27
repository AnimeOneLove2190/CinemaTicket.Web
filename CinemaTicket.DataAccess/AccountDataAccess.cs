using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CinemaTicket.Entities;
using CinemaTicket.DataAccess.Interfaces;

namespace CinemaTicket.DataAccess
{
    public class AccountDataAccess : IAccountDataAccess
    {
        private readonly CinemaManagerContext cinemaManagerContext;
        public AccountDataAccess(CinemaManagerContext cinemaManagerContext)
        {
            this.cinemaManagerContext = cinemaManagerContext;
        }
        public async Task CreateAsync(Account account)
        {
            await cinemaManagerContext.Accounts.AddAsync(account);
            await cinemaManagerContext.SaveChangesAsync();
        }
        public async Task<bool> LoginExistAsync(string login)
        {
            return await cinemaManagerContext.Accounts.FirstOrDefaultAsync(x => x.Login.ToLower() == login.ToLower()) != null;
        }
        public async Task<bool> EmailExistAsync(string email)
        {
            return await cinemaManagerContext.Accounts.FirstOrDefaultAsync(x => x.Email.ToLower() == email.ToLower()) != null;
        }
        public async Task<Account> GetAccountAsync(string login)
        {
            return await cinemaManagerContext.Accounts.Include(x => x.Role).FirstOrDefaultAsync(x => x.Login == login);
        }
        public async Task<Account> GetAccountAsync(Guid id)
        {
            return await cinemaManagerContext.Accounts.Include(x => x.Role).FirstOrDefaultAsync(x => x.Id == id);
        }
        public async Task<List<Account>> GetAccountListAsync()
        {
            return await cinemaManagerContext.Accounts.Include(x => x.Role).ToListAsync();
        }
    }
}
