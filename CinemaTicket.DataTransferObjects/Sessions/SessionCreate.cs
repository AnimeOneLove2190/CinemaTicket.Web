using System;

namespace CinemaTicket.DataTransferObjects.Sessions
{
    public class SessionCreate
    {
        public DateTime Start { get; set; }
        public int Price { get; set; }
        public int MovieId { get; set; }
        public int HallId { get; set; }
    }
}
