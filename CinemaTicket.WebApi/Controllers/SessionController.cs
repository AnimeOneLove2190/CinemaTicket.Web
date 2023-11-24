using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CinemaTicket.BusinessLogic.Interfaces;
using CinemaTicket.DataTransferObjects.Sessions;

namespace CinemaTicket.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SessionController : ControllerBase
    {
        private readonly ISessionService sessionService;
        public SessionController(ISessionService sessionService)
        {
            this.sessionService = sessionService;
        }
        [HttpPost]
        [Route("AddSession")]
        public async Task CreateSessionAsync(SessionCreate sessionCreate)
        {
            await sessionService.CreateAsync(sessionCreate);
        }
        [HttpPost]
        [Route("UpdateSession")]
        public async Task UpdateSessionAsync(SessionUpdate sessionUpdate)
        {
            await sessionService.UpdateAsync(sessionUpdate);
        }
        [HttpGet]
        [Route("GetSession")]
        public async Task<SessionDetails> GetSessionAsync(int id)
        {
            return await sessionService.GetAsync(id);
        }
        [HttpGet]
        [Route("GetSessionList")]
        public async Task<List<SessionListElement>> GetSessionListAsync()
        {
            return await sessionService.GetListAsync();
        }
        [HttpPost]
        [Route("DeleteSession")]
        public async Task DeleteSessionAsync(int id)
        {
            await sessionService.DeleteAsync(id);
        }
        [HttpGet]
        [Route("GetSeansList")]
        public async Task<List<SeansView>> GetSeansListAsync(Nullable<DateTime> start, Nullable<DateTime> end)
        {
            return await sessionService.GetSeansViewList(start, end);
        }
    }
}
