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
        public byte[] GetTemplate(string worksheetName, List<string> columns);
        public Dictionary<string, int> GetColumnIndexes(ExcelWorksheet worksheet, List<string> columns)
        ;
    }
}
