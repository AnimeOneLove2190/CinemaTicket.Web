using System.Collections.Generic;
using System.Threading.Tasks;
using CinemaTicket.DataTransferObjects.Halls;

namespace CinemaTicket.BusinessLogic.Interfaces
{
    public interface IHallService
    {
        Task CreateAsync(HallCreate hallCreate);
        Task UpdateAsync(HallUpdate hallUpdate);
        Task<HallDetails> GetAsync(int id);
        Task<List<HallListElement>> GetListAsync();
        Task DeleteAsync(int id);
    }
}
