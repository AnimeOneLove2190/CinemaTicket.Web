using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using OfficeOpenXml;
using CinemaTicket.Entities;
using CinemaTicket.Infrastructure.Constants;
using CinemaTicket.BusinessLogic.Interfaces;
using CinemaTicket.DataAccess.Interfaces;

namespace CinemaTicket.BusinessLogicServices
{
    public class ReportService
    {
        private readonly IExcelService excelService;
        private readonly ISessionDataAccess sessionDataAccess;
        public ReportService(IExcelService excelService, ISessionDataAccess sessionDataAccess)
        {
            this.excelService = excelService;
            this.sessionDataAccess = sessionDataAccess;
        }
        public async Task<byte[]> CreateIncomeReportAsync(DateTime startDate, DateTime endDate)
        {
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add(ReportTemplate.IncomeReport.WorkSheetName);
                var columns = ReportTemplate.IncomeReport.ColumnNames;
                excelService.SetHeader(worksheet, columns);
                var dataList = await GetIncomeReportData(startDate, endDate);
                excelService.SetWorkSheetData(worksheet, columns, dataList);
                worksheet.Cells.AutoFitColumns();
                var fileBytes = package.GetAsByteArray();
                return fileBytes;
            }
        }
        public async Task<List<Dictionary<string, object>>> GetIncomeReportData(DateTime startDate, DateTime endDate) // TODO установить microsoftExcel и проверить, эта неведомая хрень вообще рабоает или нет
        {
            var sessions = await sessionDataAccess.GetListForReportAsync(startDate, endDate);
            var dataList = new List<Dictionary<string, object>>();
            for (DateTime oneDay = startDate.Date; oneDay <= endDate.AddDays(1); oneDay.AddDays(1))
            {
                var sessionsInDay = sessions.Where(x => x.Start >= oneDay && x.Start <= oneDay.AddDays(1)).ToList();
                var recordDict = new Dictionary<string, object>
                {
                    [ReportTemplate.IncomeReport.Column.DateOfDay] = oneDay,
                    [ReportTemplate.IncomeReport.Column.MoviesNames] = sessionsInDay.Select(x => x.Movie.Name).Distinct().ToList(), // TODO особое внимание этой неведомой хрени
                    [ReportTemplate.IncomeReport.Column.SeansCount] = sessionsInDay.Count(),
                    [ReportTemplate.IncomeReport.Column.AmountOfIncome] = sessionsInDay.SelectMany(x => x.Tickets).Where(x => x.IsSold).Select(x => x.Price).ToList().Sum()
                };
                dataList.Add(recordDict);
            }
            return dataList;
        }
    }
}
