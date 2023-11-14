using System;
using System.Collections.Generic;
using System.Text;

namespace CinemaTicket.DataTransferObjects.Places
{
    public class PlaceUpdate
    {
        public int Id { get; set; }
        public int Capacity { get; set; }
        public int Number { get; set; }
        public int RowId { get; set; }
    }
}
