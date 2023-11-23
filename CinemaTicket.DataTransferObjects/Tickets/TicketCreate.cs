using System;
using System.Collections.Generic;
using System.Text;

namespace CinemaTicket.DataTransferObjects.Tickets
{
    public class TicketCreate
    {
        public int Price { get; set; }
        public int PlaceId { get; set; }
        public int SessionId { get; set; }
    }
}
