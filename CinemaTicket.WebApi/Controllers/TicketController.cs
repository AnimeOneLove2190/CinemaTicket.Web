using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CinemaTicket.BusinessLogic.Interfaces;
using CinemaTicket.DataTransferObjects.Tickets;

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
        public async Task CreateTicketAsync(TicketCreate ticketCreate)
        {
            await ticketService.CreateAsync(ticketCreate);
        }
        [HttpPost]
        [Route("UpdateTicket")]
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
        public async Task DeleteTicketAsync(int id)
        {
            await ticketService.DeleteAsync(id);
        }
    }
}
