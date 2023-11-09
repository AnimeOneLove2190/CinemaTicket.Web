using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CinemaTicket.BusinessLogic.Interfaces;
using CinemaTicket.DataTransferObjects.Rows;

namespace CinemaTicket.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RowController : ControllerBase
    {
        private readonly IRowService rowService;
        public RowController(IRowService rowService)
        {
            this.rowService = rowService;
        }
        [HttpPost]
        [Route("AddRow")]
        public async Task CreateRowAsync(RowCreate rowCreate)
        {
            await rowService.CreateAsync(rowCreate);
        }
        [HttpPost]
        [Route("UpdateRow")]
        public async Task UpdateRowAsync(RowUpdate rowUpdate)
        {
            await rowService.UpdateAsync(rowUpdate);
        }
    }
}
