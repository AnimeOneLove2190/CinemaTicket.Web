﻿using System;
using System.Collections.Generic;
using System.Text;

namespace CinemaTicket.DataTransferObjects.Movies
{
    public class MoviePageView
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Duration { get; set; }
        public List<string> GenreNames { get; set; }
    }
}
