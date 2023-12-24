using Microsoft.AspNetCore.Mvc;
using System;

namespace CinemaTicket.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        [HttpGet]
        [Route("info")]
        public string GetInfo()
        {
            return $"Backend is started {DateTime.Now}";
        }
    }
}
