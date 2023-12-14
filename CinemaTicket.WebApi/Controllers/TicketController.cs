using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CinemaTicket.BusinessLogic.Interfaces;
using CinemaTicket.DataTransferObjects.Tickets;
using CinemaTicket.Infrastructure.Constants;

namespace CinemaTicket.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TicketController : ControllerBase
    {
        private readonly ITicketService ticketService;
        public TicketController(ITicketService ticketService)
        {
            this.ticketService = ticketService;
        }
        [HttpPost]
        [Route("AddTicket")]
        [Authorize]
        public async Task CreateTicketAsync(TicketCreate ticketCreate)
        {
            await ticketService.CreateAsync(ticketCreate);
        }
        [HttpPost]
        [Route("UpdateTicket")]
        [Authorize]
        public async Task UpdateTicketAsync(TicketUpdate ticketUpdate)
        {
            await ticketService.UpdateAsync(ticketUpdate);
        }
        [HttpGet]
        [Route("GetTicket")]
        public async Task<TicketDetails> GetTicketAsync(int id)
        {
            return await ticketService.GetAsync(id);
        }
        [HttpGet]
        [Route("GetTicketList")]
        public async Task<List<TicketListElement>> GetTicketListAsync()
        {
            return await ticketService.GetListAsync();
        }
        [HttpPost]
        [Route("DeleteTicket")]
        [Authorize]
        public async Task DeleteTicketAsync(int id)
        {
            await ticketService.DeleteAsync(id);
        }
        [HttpGet]
        [Route("GetTicketViewList")]
        public async Task<List<TicketView>> GetTicketViewListAsync(int sessionId, bool? isSold)
        {
            return await ticketService.GetTicketViewList(sessionId, isSold);
        }
        [HttpPost]
        [Route("SellTicketList")]
        [Authorize]
        public async Task SellTicketList(List<int> ticketsIds)
        {
            await ticketService.SellTickets(ticketsIds);
        }
        [HttpPost]
        [Route("DeleteTicketList")]
        [Authorize]
        public async Task DeleteTicketList(List<int> ticketsIds)
        {
            await ticketService.DeleteTickets(ticketsIds);
        }
        [HttpGet]
        [Route("GetBulkTicketCreateTemlplate")]
        [Authorize]
        public IActionResult GetBulkTicketCreateTemplate()
        {
            var fileArray = ticketService.GetBulkTicketCreateTemplate();
            return File(fileArray, ContetnType.Excel, BulkTicketCreateTemplate.Name);
        }
        [HttpPost]
        [Route("BulkTicletCreate")]
        [Authorize]
        public async Task BulkTicketCreateTemplateAsync(IFormFile file, int sessionId)
        {
            await ticketService.BulkTicketCreate(file, sessionId);
        }
    }
}
