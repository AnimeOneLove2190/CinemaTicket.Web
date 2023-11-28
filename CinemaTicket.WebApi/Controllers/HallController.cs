using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
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
        [Authorize]
        public async Task CreateHallAsync(HallCreate hallCreate)
        {
            await hallService.CreateAsync(hallCreate);
        }
        [HttpPost]
        [Route("UpdateHall")]
        [Authorize]
        public async Task UpdateHallAsync(HallUpdate movieUpdate)
        {
            await hallService.UpdateAsync(movieUpdate);
        }
        [HttpGet]
        [Route("GetHall")]
        public async Task<HallDetails> GetHallAsync(int id)
        {
            return await hallService.GetAsync(id);
        }
        [HttpGet]
        [Route("GetHallList")]
        public async Task<List<HallListElement>> GetHallListAsync()
        {
            return await hallService.GetListAsync();
        }
        [HttpPost]
        [Route("DeleteHall")]
        [Authorize]
        public async Task DeleteHallAsync(int id)
        {
            await hallService.DeleteAsync(id);
        }
    }
}
