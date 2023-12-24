﻿using System.Collections.Generic;

namespace CinemaTicket.DataTransferObjects.Rows
{
    public class RowCreate
    {
        public int Number { get; set; }
        public int HallId { get; set; }
        public int PlaceCapacity { get; set; }
        public List<int> PlacesNumbers { get; set; }
    }
}
