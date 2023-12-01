using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using OfficeOpenXml;
using CinemaTicket.Entities;

namespace CinemaTicket.BusinessLogic.Interfaces
{
    public interface IExcelService
    {
        public void SetHeader(ExcelWorksheet worksheet, List<string> columns);
        public void SetWorkSheetData(ExcelWorksheet worksheet, List<string> columns, List<Dictionary<string, object>> dataList);
    }
}
