using System.Collections.Generic;

namespace CinemaTicket.Infrastructure.Constants
{
    public static class BulkTicketCreateTemplate
    {
        public const string Name = "Bulk Ticket Create Template.xlsx";
        public const string WorksheetName = "Tickets";
        public static readonly List<string> ColumnNames = new List<string> { Column.RowNumber, Column.PlaceNumber, Column.Price };
        public static class Column
        {
            public const string RowNumber = "RowNumber";
            public const string PlaceNumber = "PlaceNumber";
            public const string Price = "Price";

        }
    }
}