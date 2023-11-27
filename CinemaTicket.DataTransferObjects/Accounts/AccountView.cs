using System;
using System.Collections.Generic;
using System.Text;

namespace CinemaTicket.DataTransferObjects.Accounts
{
    public class AccountView
    {
        public Guid Id { get; set; }
        public string Login { get; set; }
        public string Email { get; set; }
        public Guid RoleId { get; set; }
        public string RoleName { get; set; }
    }
}
