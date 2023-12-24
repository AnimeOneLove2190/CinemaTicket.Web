using System.Collections.Generic;

namespace CinemaTicket.DataTransferObjects.Halls
{
    public class HallCreate
    {
        public string Name { get; set; }
        public List<int> RowsNumbers { get; set; }
    }
}
