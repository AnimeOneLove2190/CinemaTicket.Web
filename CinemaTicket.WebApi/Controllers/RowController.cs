﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
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
        [Authorize]
        public async Task CreateRowAsync(RowCreate rowCreate)
        {
            await rowService.CreateAsync(rowCreate);
        }
        [HttpPost]
        [Route("UpdateRow")]
        [Authorize]
        public async Task UpdateRowAsync(RowUpdate rowUpdate)
        {
            await rowService.UpdateAsync(rowUpdate);
        }
        [HttpGet]
        [Route("GetRow")]
        public async Task<RowDetails> GetRowAsync(int id)
        {
            return await rowService.GetAsync(id);
        }
        [HttpGet]
        [Route("GetRowList")]
        public async Task<List<RowListElement>> GetRowListAsync()
        {
            return await rowService.GetListAsync();
        }
        [HttpPost]
        [Route("DeleteRow")]
        [Authorize]
        public async Task DeleteRowAsync(int id)
        {
            await rowService.DeleteAsync(id);
        }
    }
}
