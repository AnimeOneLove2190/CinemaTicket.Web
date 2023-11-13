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
        Task UpdateAsync(PlaceUpdate placeUpdate);
        Task<PlaceDetails> GetAsync(int id);
        Task<List<PlaceListElement>> GetListAsync();
        Task DeleteAsync(int id);
    }
}
