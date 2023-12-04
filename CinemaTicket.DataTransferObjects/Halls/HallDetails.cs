using System;
using System.Collections.Generic;
using System.Text;
using CinemaTicket.DataTransferObjects.Rows;

namespace CinemaTicket.DataTransferObjects.Halls
{
    public class HallDetails
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ModifiedOn { get; set; }
        public ICollection<RowListElement> Rows { get; set; }
    }
}
