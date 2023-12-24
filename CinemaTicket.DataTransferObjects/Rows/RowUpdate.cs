using System.Collections.Generic;

namespace CinemaTicket.DataTransferObjects.Rows
{
    public class RowUpdate
    {
        public int Id { get; set; }
        public int Number { get; set; }
        public int PlaceCapacity { get; set; }
        public int HallId { get; set; }
        public List<int> PlacesNumbers { get; set; }
    }
}
