﻿using System;
using System.Collections.Generic;
using System.Text;
using CinemaTicket.DataTransferObjects.Places;

namespace CinemaTicket.DataTransferObjects.Rows
{
    public class RowDetails
    {
        public int Id { get; set; }
        public int Number { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ModifiedOn { get; set; }
        public int HallId { get; set; }
        public ICollection<PlaceDetails> Places { get; set; }
    }
}
