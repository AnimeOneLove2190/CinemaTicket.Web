using System;
using System.Collections.Generic;
using System.Text;

namespace CinemaTicket.DataTransferObjects.Halls
{
    public class HallCreate
    {
        public string Name { get; set; }
        public List<int> RowsNumbers { get; set; }
    }
}
