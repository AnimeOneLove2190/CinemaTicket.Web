using System;

namespace CinemaTicket.DataTransferObjects.Sessions
{
    public class SeansView
    {
        public int Id { get; set; }
        public string MovieName { get; set; }
        public int HallId { get; set; }
        public DateTime Start { get; set; }
        public int Duration { get; set; }
        public bool HasTickets { get; set; }
        public bool HasUnsoldTickets { get; set; }
    }
}
