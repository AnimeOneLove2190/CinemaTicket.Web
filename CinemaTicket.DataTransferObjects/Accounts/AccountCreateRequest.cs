using System;
using System.Collections.Generic;
using System.Text;

namespace CinemaTicket.DataTransferObjects.Accounts
{
    public class AccountCreateRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string Login { get; set; }
    }
}
