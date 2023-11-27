using System;
using System.Collections.Generic;
using System.Text;

namespace CinemaTicket.Infrastructure.Constants
{
    public class Permissions
    {
        public static class Id
        {
            public static readonly Guid Admin = new Guid("3868C13D-8D12-46A6-B709-52BCF010BDFF");
            public static readonly Guid DefaultUser = new Guid("375AF7CC-A281-4432-A3F5-14AF10BF73F6");
        }
    }
}
