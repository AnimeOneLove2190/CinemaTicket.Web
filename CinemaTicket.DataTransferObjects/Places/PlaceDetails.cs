using System;
using System.Collections.Generic;
using CinemaTicket.DataTransferObjects.Tickets;

namespace CinemaTicket.DataTransferObjects.Places
{
    public class PlaceDetails
    {
        public int Id { get; set; }
        public int Capacity { get; set; }
        public int Number { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ModifiedOn { get; set; }
        public int RowId { get; set; }
        public ICollection<TicketDetails> Tickets { get; set; }
    }
}
