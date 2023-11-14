using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CinemaTicket.Entities;
using CinemaTicket.DataTransferObjects.Sessions;

namespace CinemaTicket.BusinessLogic.Interfaces
{
    public interface ISessionService
    {
        Task CreateAsync(SessionCreate sessionCreate);
    }
}
