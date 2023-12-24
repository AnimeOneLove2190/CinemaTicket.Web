using System.Collections.Generic;
using OfficeOpenXml;

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
