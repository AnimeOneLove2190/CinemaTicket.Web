
namespace CinemaTicket.DataTransferObjects.Tickets
{
    public class TicketView
    {
        public int Id { get; set; }
        public string MovieName { get; set; }
        public int PlaceNumber { get; set; }
        public int RowNumber { get; set; }
        public int HallId { get; set; }
        public bool IsSold { get; set; }
        public int Price { get; set; }
    }
}
