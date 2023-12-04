using System;
using System.Collections.Generic;
using System.Text;

namespace CinemaTicket.Infrastructure.Constants
{
    public static class ReportTemplate
    {
        public static class IncomeReport
        {
            public const string WorkSheetName = "Income";
            public static readonly List<string> ColumnNames = new List<string>
            {
                Column.DateOfDay,
                Column.MoviesNames,
                Column.SeansCount,
                Column.AmountOfIncome
            };
            public static class Column
            {
                public const string DateOfDay = "Date";
                public const string MoviesNames = "MoviesNames";
                public const string SeansCount = "SeansCount";
                public const string AmountOfIncome = "AmountOfIncome";
            }
        }
    }
}
