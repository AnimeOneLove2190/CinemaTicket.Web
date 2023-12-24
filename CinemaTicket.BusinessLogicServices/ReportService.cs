using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OfficeOpenXml;
using CinemaTicket.Infrastructure.Constants;
using CinemaTicket.BusinessLogic.Interfaces;
using CinemaTicket.DataAccess.Interfaces;

namespace CinemaTicket.BusinessLogicServices
{
    public class ReportService : IReportService
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
        public async Task<List<Dictionary<string, object>>> GetIncomeReportData(DateTime startDate, DateTime endDate)
        {
            var sessions = await sessionDataAccess.GetListForReportAsync(startDate, endDate);
            var dataList = new List<Dictionary<string, object>>();
            var endDatePlus = endDate.AddDays(1);
            for (DateTime oneDay = startDate.Date; oneDay < endDatePlus; oneDay = oneDay.AddDays(1))
            {
                var sessionsInDay = sessions.Where(x => x.Start >= oneDay && x.Start <= oneDay.AddDays(1)).ToList();
                var recordDict = new Dictionary<string, object>
                {
                    [ReportTemplate.IncomeReport.Column.DateOfDay] = oneDay.ToString("dd.MM.yyyy"),
                    [ReportTemplate.IncomeReport.Column.MoviesNames] = string.Join(',', sessionsInDay.Select(x => x.Movie.Name).Distinct().ToList()),
                    [ReportTemplate.IncomeReport.Column.SeansCount] = sessionsInDay.Count(),
                    [ReportTemplate.IncomeReport.Column.AmountOfIncome] = sessionsInDay.SelectMany(x => x.Tickets).Where(x => x.IsSold).Select(x => x.Price).ToList().Sum()
                };
                dataList.Add(recordDict);
            }
            return dataList;
        }
    }
}
