using System;
using System.Collections.Generic;
using CinemaTicket.DataTransferObjects.Tickets;

namespace CinemaTicket.DataTransferObjects.Sessions
{
    public class SessionDetails
    {
        public int Id { get; set; }
        public DateTime Start { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ModifiedOn { get; set; }
        public int MovieId { get; set; }
        public int HallId { get; set; }
        public ICollection<TicketDetails> Tickets { get; set; }
    }
}
