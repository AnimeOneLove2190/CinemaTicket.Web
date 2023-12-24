using System;

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
