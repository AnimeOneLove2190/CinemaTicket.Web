using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CinemaTicket.BusinessLogic.Interfaces;
using CinemaTicket.DataTransferObjects.Halls;

namespace CinemaTicket.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HallController : ControllerBase
    {
        private readonly IHallService hallService;
        public HallController(IHallService hallService)
        {
            this.hallService = hallService;
        }
        [HttpPost]
        [Route("AddHall")]
        public async Task CreateHallAsync(HallCreate hallCreate)
        {
            await hallService.CreateAsync(hallCreate);
        }
        [HttpPost]
        [Route("UpdateHall")]
        public async Task UpdateHallAsync(HallUpdate movieUpdate)
        {
            await hallService.UpdateAsync(movieUpdate);
        }
    }
}
