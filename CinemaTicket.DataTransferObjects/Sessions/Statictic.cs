﻿using System;
using System.Collections.Generic;
using System.Text;

namespace CinemaTicket.DataTransferObjects.Sessions
{
    public class Statictic //TODO после мержа 609/dev перенести эту хуйню в папку statistic
    {
        public int Id { get; set; }
        public string MovieName { get; set; }
        public int HallId { get; set; }
        public DateTime Start { get; set; }
        public int Duration { get; set; }
        public bool HasTickets { get; set; }
        public bool HasUnsoldTickets { get; set; }
    }
}
