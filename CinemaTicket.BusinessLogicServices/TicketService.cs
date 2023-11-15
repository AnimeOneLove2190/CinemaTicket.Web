using System;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using CinemaTicket.Entities;
using CinemaTicket.BusinessLogic.Interfaces;
using CinemaTicket.DataTransferObjects.Sessions;
using CinemaTicket.DataTransferObjects.Tickets;
using CinemaTicket.DataAccess.Interfaces;

namespace CinemaTicket.BusinessLogicServices
{
    public class TicketService : ITicketService
    {
        private readonly ISessionDataAccess sessionDataAccess;
        private readonly IPlaceDataAccess placeDataAccess;
        private readonly ITicketDataAccess ticketDataAccess;
        private readonly IRowDataAccess rowDataAccess;

        public TicketService(ISessionDataAccess sessionDataAccess, IRowDataAccess rowDataAccess, IPlaceDataAccess placeDataAccess, IMovieDataAccess movieDataAccess, ITicketDataAccess ticketDataAccess)
        {
            this.sessionDataAccess = sessionDataAccess;
            this.placeDataAccess = placeDataAccess;
            this.ticketDataAccess = ticketDataAccess;
            this.rowDataAccess = rowDataAccess;
        }
        public async Task CreateAsync(TicketCreate ticketCreate)
        {
            if (ticketCreate == null)
            {
                throw new Exception();
            }
            if (ticketCreate.Price <= 0)
            {
                throw new Exception();
            }
            var placeFromDB = await placeDataAccess.GetPlaceAsync(ticketCreate.PlaceId);
            if (placeFromDB == null)
            {
                throw new Exception();
            }
            var sessionFromDB = await sessionDataAccess.GetSessionAsync(ticketCreate.SessionId);
            if (sessionFromDB == null)
            {
                throw new Exception();
            }
            var tickeDuplicate = sessionFromDB.Tickets.FirstOrDefault(x => x.PlaceId == ticketCreate.PlaceId);
            if (tickeDuplicate != null)
            {
                throw new Exception();
            }
            var ticket = new Ticket
            {
                IsSold = false,
                DateOfSale = null,
                Price = ticketCreate.Price,
                CreatedOn = DateTime.UtcNow,
                ModifiedOn = DateTime.UtcNow,
                PlaceId = ticketCreate.PlaceId,
                SessionId = ticketCreate.SessionId,
            };
            await ticketDataAccess.CreateAsync(ticket);
        }
        public async Task UpdateAsync(TicketUpdate ticketUpdate)
        {
            if (ticketUpdate == null)
            {
                throw new Exception();
            }
            if (ticketUpdate.Id <= 0)
            {
                throw new Exception();
            }
            if (ticketUpdate.Price <= 0)
            {
                throw new Exception();
            }
            var placeFromDB = await placeDataAccess.GetPlaceAsync(ticketUpdate.PlaceId);
            if (placeFromDB == null)
            {
                throw new Exception();
            }
            var sessionFromDB = await sessionDataAccess.GetSessionAsync(ticketUpdate.SessionId);
            if (sessionFromDB == null)
            {
                throw new Exception();
            }
            var rowFromDB = await rowDataAccess.GetRowAsync(placeFromDB.RowId);
            if (sessionFromDB.HallId != rowFromDB.HallId)
            {
                throw new Exception();
            }
            var ticketFromDB = await ticketDataAccess.GetTicketAsync(ticketUpdate.Id);
            if (ticketFromDB == null)
            {
                throw new Exception();
            }
            if (ticketFromDB.IsSold == true)
            {
                throw new Exception();
            }
            ticketFromDB.IsSold = ticketUpdate.IsSold;
            if (ticketUpdate.IsSold == true)
            {
                ticketFromDB.DateOfSale = DateTime.UtcNow;
            }
            ticketFromDB.Price = ticketUpdate.Price;
            ticketFromDB.ModifiedOn = DateTime.UtcNow;
            ticketFromDB.PlaceId = ticketUpdate.PlaceId;
            ticketFromDB.SessionId = ticketUpdate.SessionId;
            await ticketDataAccess.UpdateTicketAsync(ticketFromDB);
        }
    }
}
