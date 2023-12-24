
namespace CinemaTicket.DataTransferObjects.Tickets
{
    public class TicketUpdate
    {
        public int Id { get; set; }
        public bool IsSold { get; set; }
        public int Price { get; set; }
        public int PlaceId { get; set; }
        public int SessionId { get; set; }
    }
}
