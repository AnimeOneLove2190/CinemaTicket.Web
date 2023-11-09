using System;
using System.Collections.Generic;
using System.Text;

namespace CinemaTicket.DataTransferObjects.Halls
{
    public class HallUpdate
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<int> RowsNumbers { get; set; }
    }
}
