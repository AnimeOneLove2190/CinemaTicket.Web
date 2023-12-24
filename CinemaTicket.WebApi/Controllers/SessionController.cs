using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CinemaTicket.BusinessLogic.Interfaces;
using CinemaTicket.DataTransferObjects.Sessions;
using CinemaTicket.Infrastructure.Constants;

namespace CinemaTicket.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SessionController : ControllerBase
    {
        private readonly ISessionService sessionService;
        private readonly IReportService reportService;
        public SessionController(ISessionService sessionService, IReportService reportService)
        {
            this.sessionService = sessionService;
            this.reportService = reportService;
        }
        [HttpPost]
        [Route("AddSession")]
        [Authorize]
        public async Task CreateSessionAsync(SessionCreate sessionCreate)
        {
            await sessionService.CreateAsync(sessionCreate);
        }
        [HttpPost]
        [Route("UpdateSession")]
        [Authorize]
        public async Task UpdateSessionAsync(SessionUpdate sessionUpdate)
        {
            await sessionService.UpdateAsync(sessionUpdate);
        }
        [HttpGet]
        [Route("GetSession")]
        public async Task<SessionDetails> GetSessionAsync(int id)
        {
            return await sessionService.GetAsync(id);
        }
        [HttpGet]
        [Route("GetSessionList")]
        public async Task<List<SessionListElement>> GetSessionListAsync()
        {
            return await sessionService.GetListAsync();
        }
        [HttpPost]
        [Route("DeleteSession")]
        [Authorize]
        public async Task DeleteSessionAsync(int id)
        {
            await sessionService.DeleteAsync(id);
        }
        [HttpGet]
        [Route("GetSeansList")]
        public async Task<List<SeansView>> GetSeansListAsync(DateTime? start, DateTime? end)
        {
            return await sessionService.GetSeansViewList(start, end);
        }
        [HttpGet]
        [Route("GetReport")]
        [Authorize]
        public async Task<IActionResult> GetReportAsync(DateTime starDate, DateTime endDate)
        {
            var fileArray = await reportService.CreateIncomeReportAsync(starDate, endDate);
            return File(fileArray, ContetnType.Excel, ReportNames.IncomeReport);
        }
    }
}
