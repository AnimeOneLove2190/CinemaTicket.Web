using System;

namespace CinemaTicket.Entities
{
    public class Role : IDbEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
