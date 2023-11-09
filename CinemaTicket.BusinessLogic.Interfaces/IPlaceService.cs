using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CinemaTicket.Entities;
using CinemaTicket.DataTransferObjects.Places;

namespace CinemaTicket.BusinessLogic.Interfaces
{
    public interface IPlaceService
    {
        Task CreateAsync(PlaceCreate placeCreate);
    }
}
