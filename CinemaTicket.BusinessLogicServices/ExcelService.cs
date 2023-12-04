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
    }
}
