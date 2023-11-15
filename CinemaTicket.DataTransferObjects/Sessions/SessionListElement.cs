using System;
using System.Collections.Generic;
using System.Text;

namespace CinemaTicket.DataTransferObjects.Sessions
{
    public class SessionListElement
    {
        public int Id { get; set; }
        public DateTime Start { get; set; }
        public int MovieId { get; set; }
        public int HallId { get; set; }
    }
}
