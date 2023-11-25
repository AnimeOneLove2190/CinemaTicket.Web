using System;
using System.Collections.Generic;
using System.Text;

namespace CinemaTicket.Infrastructure.Exceptions
{
    public class CustomDevExceptionDetails : CustomExceptionDetails
    {
        public string StackTrace { get; set; }
    }
}
