﻿using System.Collections.Generic;

namespace CinemaTicket.DataTransferObjects.Movies
{
    public class MovieUpdate
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Duration { get; set; }
        public List<int> GenreIds { get; set; }
    }
}
