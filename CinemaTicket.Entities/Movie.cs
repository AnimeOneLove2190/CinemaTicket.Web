using System;
using System.Collections.Generic;
using System.Text;

namespace CinemaTicket.Entities
{
    public class Movie
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Duration { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ModifiedOn { get; set; }
        public ICollection<Session> Sessions { get; set; }
        public ICollection<Genre> Genres { get; set; }
    }
}
