using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations.Schema;

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
        public Guid CreatedBy { get; set; }
        [ForeignKey("CreatedBy")]
        public Account CreatedByUser { get; set; }
        public Guid ModifiedBy { get; set; }
        [ForeignKey("ModifiedBy")]
        public Account ModifiedByUser { get; set; }
        public ICollection<Ticket> Tickets { get; set; }
    }
}
