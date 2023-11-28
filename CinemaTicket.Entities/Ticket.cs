using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations.Schema;

namespace CinemaTicket.Entities
{
    public class Ticket : IDbEntity
    {
        public int Id { get; set; }
        public bool IsSold { get; set; }
        public Nullable<DateTime> DateOfSale { get; set; }
        public int Price { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ModifiedOn { get; set; }
        public int PlaceId { get; set; }
        public Place Place { get; set; }
        public int SessionId { get; set; }
        public Session Session { get; set; }
        public Guid CreatedBy { get; set; }
        [ForeignKey("CreatedBy")]
        public Account CreatedByUser { get; set; }
        public Guid ModifiedBy { get; set; }
        [ForeignKey("ModifiedBy")]
        public Account ModifiedByUser { get; set; }
    }
}
