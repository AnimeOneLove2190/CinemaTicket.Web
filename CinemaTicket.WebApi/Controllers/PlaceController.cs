using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CinemaTicket.BusinessLogic.Interfaces;
using CinemaTicket.DataTransferObjects.Places;

namespace CinemaTicket.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlaceController : ControllerBase
    {
        private readonly IPlaceService placeService;
        public PlaceController(IPlaceService placeService)
        {
            this.placeService = placeService;
        }
        [HttpPost]
        [Route("AddPlace")]
        public async Task CreatePlaceAsync(PlaceCreate placeCreate)
        {
            await placeService.CreateAsync(placeCreate);
        }
    }
}
