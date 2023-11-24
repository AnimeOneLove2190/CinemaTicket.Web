using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CinemaTicket.BusinessLogic.Interfaces;
using CinemaTicket.DataTransferObjects.Sessions;
using CinemaTicket.DataTransferObjects.Tickets;

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
        public async Task<List<SeansView>> GetSeansListAsync(Nullable<DateTime> start, Nullable<DateTime> end)
        {
            return await statisticService.GetSeansViewList(start, end);
        }
        [HttpGet]
        [Route("GetTicketStatisticList")]
        public async Task<List<TicketView>> GetTicketStatisticListAsync(int sessionId, Nullable<bool> isSold)
        {
            return await statisticService.GetTicketViewList(sessionId, isSold);
        }
        [HttpPost]
        [Route("SellTicketList")]
        public async Task SellTicketList(List<int> ticketsIds)
        {
            await statisticService.SellTickets(ticketsIds);
        }
        [HttpPost]
        [Route("DeleteTicketList")]
        public async Task DeleteTicketList(List<int> ticketsIds)
        {
            await statisticService.DeleteTickets(ticketsIds);
        }
    }
}
