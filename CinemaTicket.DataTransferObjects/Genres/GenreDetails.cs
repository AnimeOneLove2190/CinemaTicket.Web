using System;
using System.Collections.Generic;
using System.Text;

namespace CinemaTicket.DataTransferObjects.Genres
{
    public class GenreDetails
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ModifiedOn { get; set; }
    }
}
