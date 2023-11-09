using System;
using System.Collections.Generic;
using System.Text;

namespace CinemaTicket.DataTransferObjects.Rows
{
    public class RowUpdate
    {
        public int Id { get; set; }
        public int Number { get; set; }
        public int HallId { get; set; }
        public List<int> PlacesNumbers { get; set; }
    }
}
