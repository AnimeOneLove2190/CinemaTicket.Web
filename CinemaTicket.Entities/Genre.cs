using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace CinemaTicket.Entities
{
    public class Genre : IDbEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ModifiedOn { get; set; }
        public Guid CreatedBy { get; set; }
        [ForeignKey("CreatedBy")]
        public Account CreatedByUser { get; set; }
        public Guid ModifiedBy { get; set; }
        [ForeignKey("ModifiedBy")]
        public Account ModifiedByUser { get; set; }
        public ICollection<Movie> Movies { get; set; }
    }
}
