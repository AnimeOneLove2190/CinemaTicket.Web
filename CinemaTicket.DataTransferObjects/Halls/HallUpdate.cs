using System.Collections.Generic;

namespace CinemaTicket.DataTransferObjects.Halls
{
    public class HallUpdate
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<int> RowsNumbers { get; set; }
    }
}
