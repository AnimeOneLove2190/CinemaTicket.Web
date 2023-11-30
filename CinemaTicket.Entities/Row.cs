using System;
using System.Collections.Generic;
using System.Text;

namespace CinemaTicket.Entities
{
    public class Row : IDbEntity
    {
        public int Id { get; set; }
        public int Number { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ModifiedOn { get; set; }
        public int HallId { get; set; }
        public Hall Hall { get; set; }
        public ICollection<Place> Places { get; set; }
    }
}
