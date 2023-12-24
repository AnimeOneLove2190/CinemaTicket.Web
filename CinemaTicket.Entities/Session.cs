using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace CinemaTicket.Entities
{
    public class Session : IDbEntity
    {
        public int Id { get; set; }
        public DateTime Start { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ModifiedOn { get; set; }
        public int MovieId { get; set; }
        public Movie Movie { get; set; }
        public int HallId { get; set; }
        public Hall Hall { get; set; }
        public Guid CreatedBy { get; set; }
        [ForeignKey("CreatedBy")]
        public Account CreatedByUser { get; set; }
        public Guid ModifiedBy { get; set; }
        [ForeignKey("ModifiedBy")]
        public Account ModifiedByUser { get; set; }
        public ICollection<Ticket> Tickets { get; set; }
    }
}
