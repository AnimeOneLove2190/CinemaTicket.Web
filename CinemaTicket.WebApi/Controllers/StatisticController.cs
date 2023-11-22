using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CinemaTicket.BusinessLogic.Interfaces;
using CinemaTicket.DataTransferObjects.Sessions;
using CinemaTicket.DataTransferObjects.Statistic;

namespace CinemaTicket.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatisticController : ControllerBase
    {
        private readonly IStatisticService statisticService;
        public StatisticController(IStatisticService statisticService)
        {
            this.statisticService = statisticService;
        }
        [HttpGet]
        [Route("GetSeansList")]
        public async Task<List<Statictic>> GetSeansListAsync(Nullable<DateTime> start, Nullable<DateTime> end)
        {
            return await statisticService.GetStaticticList(start, end);
        }
        [HttpGet]
        [Route("GetTicketStatisticList")]
        public async Task<List<TicketStatistic>> GetTicketStatisticListAsync(int sessionId, Nullable<bool> isSold)
        {
            return await statisticService.GetTicketStaticticList(sessionId, isSold);
        }
    }
}
