using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CinemaTicket.DataAccess;
using CinemaTicket.Entities;

namespace CinemaTicket.DataAccess.Interfaces
{
    public interface IAccountDataAccess
    {
        Task CreateAsync(Account account);
        Task<bool> LoginExistAsync(string login);
        Task<bool> EmailExistAsync(string email);
    }
}
