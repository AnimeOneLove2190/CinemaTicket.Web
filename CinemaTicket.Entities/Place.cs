using System;
using System.Collections.Generic;
using System.Text;

namespace CinemaTicket.Entities
{
    public class Place : IDbEntity
    {
        public int Id { get; set; }
        public int Capacity { get; set; }
        public int Number { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ModifiedOn { get; set; }
        public int RowId { get; set; }
        public Row Row { get; set; }
        public ICollection<Ticket> Tickets { get; set; }
    }
}
