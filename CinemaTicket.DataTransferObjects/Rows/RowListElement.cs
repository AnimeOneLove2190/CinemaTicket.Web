using System;
using System.Collections.Generic;
using System.Text;
using CinemaTicket.DataTransferObjects.Places;

namespace CinemaTicket.DataTransferObjects.Rows
{
    public class RowListElement
    {
        public int Id { get; set; }
        public int Number { get; set; }
        public int HallId { get; set; }
        public ICollection<PlaceListElement> Places { get; set; }
    }
}
