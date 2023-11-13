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
        [HttpGet]
        [Route("GetPlace")]
        public async Task<PlaceDetails> GetPlaceAsync(int id)
        {
            return await placeService.GetAsync(id);
        }
        [HttpGet]
        [Route("GetPlaceList")]
        public async Task<List<PlaceListElement>> GetPlaceListAsync()
        {
            return await placeService.GetListAsync();
        }
        [HttpPost]
        [Route("DeletePlace")]
        public async Task DeletePlaceAsync(int id)
        {
            await placeService.DeleteAsync(id);
        }
    }
}
