using System.Collections.Generic;

namespace CinemaTicket.DataTransferObjects.Movies
{
    public class MovieCreate
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Duration { get; set; }
        public List<string> GenreNames { get; set; }
    }
}
