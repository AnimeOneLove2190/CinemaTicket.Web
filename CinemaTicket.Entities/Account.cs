using System;
using System.Collections.Generic;

namespace CinemaTicket.Entities
{
    public class Account : IDbEntity
    {
        public Guid Id { get; set; }
        public string Login { get; set; }
        public string HashPassword { get; set; }
        public Guid RoleId { get; set; }
        public Role Role { get; set; }
        public string Email { get; set; }
        public DateTime CreatedOn { get; set; }
        public ICollection<Genre> CreatedGenres { get; set; }
        public ICollection<Genre> ModifiedGenres { get; set; }
        public ICollection<Hall> CreatedHalls { get; set; }
        public ICollection<Hall> ModifiedHalls { get; set; }
        public ICollection<Movie> CreatedMovies { get; set; }
        public ICollection<Movie> ModifiedMovies { get; set; }
        public ICollection<Place> CreatedPlaces { get; set; }
        public ICollection<Place> ModifiedPlaces { get; set; }
        public ICollection<Row> CreatedRows { get; set; }
        public ICollection<Row> ModifiedRows { get; set; }
        public ICollection<Session> CreatedSessions { get; set; }
        public ICollection<Session> ModifiedSessions { get; set; }
        public ICollection<Ticket> CreatedTickets { get; set; }
        public ICollection<Ticket> ModifiedTickets { get; set; }

    }
}
