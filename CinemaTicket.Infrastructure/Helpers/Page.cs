using System.Collections.Generic;

namespace CinemaTicket.Infrastructure.Helpers
{
    public class Page<T>
    {
        public List<T> Items { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int Total { get; set; }
    }
}
