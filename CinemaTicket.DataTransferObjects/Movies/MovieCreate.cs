using System;

namespace CinemaTicket.DataTransferObjects.Movies
{
    public class MovieCreate
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Duration { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ModifiedOn { get; set; }
    }
}
