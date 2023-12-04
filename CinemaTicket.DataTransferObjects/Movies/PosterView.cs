using System;
using System.Collections.Generic;
using System.Text;

namespace CinemaTicket.DataTransferObjects.Movies
{
    public class PosterView
    {
        public byte[] fileArray { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
    }
}
