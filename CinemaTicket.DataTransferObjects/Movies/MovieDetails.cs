using System;
using System.Collections.Generic;
using CinemaTicket.DataTransferObjects.Genres;

namespace CinemaTicket.DataTransferObjects.Movies
{
    public class MovieDetails
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Duration { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ModifiedOn { get; set; }
        public ICollection<GenreDetails> Genres { get; set; }
    }
}
