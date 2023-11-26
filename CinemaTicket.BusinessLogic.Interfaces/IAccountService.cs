using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CinemaTicket.DataTransferObjects.Accounts;

namespace CinemaTicket.BusinessLogic.Interfaces
{
    public interface IAccountService
    {
        Task CreateAccountAsync(AccountCreateRequest accountCreateRequest);
    }
}
