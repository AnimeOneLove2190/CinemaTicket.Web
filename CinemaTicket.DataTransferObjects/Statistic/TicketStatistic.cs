using System;
using System.Collections.Generic;
using System.Text;

namespace CinemaTicket.DataTransferObjects.Statistic
{
    public class TicketStatistic
    {
        public int Id { get; set; }
        public string MovieName { get; set; }
        public int PlaceNumber { get; set; }
        public int RowNumber { get; set; }
        public int HallId { get; set; }
        public bool IsSold { get; set; }
        public int Price { get; set; }
    }
}
