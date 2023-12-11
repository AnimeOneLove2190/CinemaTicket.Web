using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using CinemaTicket.Infrastructure.Constants;
using CinemaTicket.Infrastructure.Exceptions;
using CinemaTicket.BusinessLogic.Interfaces;

namespace CinemaTicket.BusinessLogicServices
{
    public class ExcelService : IExcelService
    {
        private ILogger<ExcelService> logger;
        public ExcelService(ILogger<ExcelService> logger)
        {
            this.logger = logger;
        }
        public void SetHeader(ExcelWorksheet worksheet, List<string> columns)
        {
            var headerRawIndex = 1;
            var firstColumnIndex = 1;
            for (int i = 0; i < columns.Count; i++)
            {
                worksheet.Cells[headerRawIndex, i + firstColumnIndex].Value = columns[i];
            }
        }
        public void SetWorkSheetData (ExcelWorksheet worksheet, List<string> columns, List<Dictionary<string, object>> dataList)
        {
            var headerRawIndex = 1;
            var dataRawIndex = headerRawIndex + 1;
            var firstColumnIndex = 1;
            for (int i = 0; i < dataList.Count; i++)
            {
                for (int j = 0; j < columns.Count; j++)
                {
                    worksheet.Cells[i + dataRawIndex, j + firstColumnIndex].Value = dataList[i][columns[j]];
                }
            }
        }
        public byte[] GetTemplate(string worksheetName, List<string> columns)
        {
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add(worksheetName);
                SetHeader(worksheet, columns);
                worksheet.Cells.AutoFitColumns();
                var fileBytes = package.GetAsByteArray();
                return fileBytes;
            }
        }
        public Dictionary<string, int> GetColumnIndexes(ExcelWorksheet worksheet, List<string> columns)
        {
            var columnIndexes = new Dictionary<string, int>();
            foreach (var columnName in columns)
            {
                var columnIndex = FindColumnIndex(worksheet, columnName);
                if(columnIndex != -1)
                {
                    columnIndexes.Add(columnName, columnIndex);
                }
                else
                {
                    var exceptionMessage = string.Format(ExceptionMessageTemplate.ColumnNotFound, columnName);
                    logger.LogError(exceptionMessage);
                    throw new NotFoundException(exceptionMessage);
                }
            }
            return columnIndexes;
        }
        public int FindColumnIndex(ExcelWorksheet worksheet, string columnName)
        {
            int totalColumns = worksheet.Dimension.Columns;
            for (int col = 1; col <= totalColumns; col++)
            {
                var cellValue = worksheet.Cells[1, col].Text;
                if (string.Equals(cellValue, columnName, StringComparison.OrdinalIgnoreCase))
                {
                    return col;
                }
            }
            return -1;
        }
    }
}
