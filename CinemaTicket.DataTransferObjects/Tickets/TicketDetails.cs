using System;

namespace CinemaTicket.DataTransferObjects.Tickets
{
    public class TicketDetails
    {
        public int Id { get; set; }
        public bool IsSold { get; set; }
        public Nullable<DateTime> DateOfSale { get; set; }
        public int Price { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ModifiedOn { get; set; }
        public int PlaceId { get; set; }
        public int SessionId { get; set; }
    }
}
