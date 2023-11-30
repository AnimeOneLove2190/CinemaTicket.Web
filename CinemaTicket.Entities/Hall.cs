using System;
using System.Collections.Generic;

namespace CinemaTicket.Entities
{
    public class Hall : IDbEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ModifiedOn { get; set; }
        public ICollection<Row> Rows { get; set; }
        public ICollection<Session> Sessions { get; set; }
    }
}
