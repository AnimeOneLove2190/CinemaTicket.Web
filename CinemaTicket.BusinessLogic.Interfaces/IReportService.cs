using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CinemaTicket.BusinessLogic.Interfaces
{
    public interface IReportService
    {
        Task<byte[]> CreateIncomeReportAsync(DateTime startDate, DateTime endDate);
        Task<List<Dictionary<string, object>>> GetIncomeReportData(DateTime startDate, DateTime endDate);
    }
}
