using System.Collections.Generic;
using System.Threading.Tasks;
using CinemaTicket.Entities;

namespace CinemaTicket.DataAccess.Interfaces
{
    public interface IPlaceDataAccess : IBaseDataAccess
    {
        Task<Place> GetPlaceAsync(int id);
        Task<List<Place>> GetPlaceListAsync();
        Task<List<Place>> GetPlaceListAsync(List<int> placesIds);
    }
}
