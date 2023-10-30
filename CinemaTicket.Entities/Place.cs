using System;
using System.Collections.Generic;
using System.Text;

namespace CinemaTicket.Entities
{
    public class Place
    {
        public int Id { get; set; }
        public int Capacity { get; set; }
        public int Number { get; set; }
        public int RowId { get; set; }
        public Row Row { get; set; }
        public ICollection<Ticket> Tickets { get; set; }
    }
}
