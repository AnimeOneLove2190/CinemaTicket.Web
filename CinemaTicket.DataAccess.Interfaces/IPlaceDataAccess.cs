using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CinemaTicket.Entities;

namespace CinemaTicket.DataAccess.Interfaces
{
    public interface IPlaceDataAccess
    {
        Task CreateAsync(Place place);
        Task CreateAsync(List<Place> places);
        Task<Place> GetPlaceAsync(int id);
        Task<List<Place>> GetPlaceListAsync();
        Task<List<Place>> GetPlaceListAsync(List<int> placesIds);
        Task UpdatePlaceAsync(Place place);
        Task DeletePlaceAsync(Place place);
    }
}
