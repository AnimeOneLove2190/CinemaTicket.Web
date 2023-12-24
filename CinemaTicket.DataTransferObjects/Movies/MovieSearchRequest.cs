using System.Collections.Generic;

namespace CinemaTicket.DataTransferObjects.Movies
{
    public class MovieSearchRequest
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string MovieName { get; set; }
        public string Description { get; set; }
        public int? MinDuration { get; set; }
        public int? MaxDuration { get; set; }
        public List<int> GenreIds { get; set; }
    }
}
