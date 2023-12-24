using System.Collections.Generic;
using System.Threading.Tasks;
using CinemaTicket.Entities;

namespace CinemaTicket.DataAccess.Interfaces
{
    public interface IRowDataAccess : IBaseDataAccess
    {
        Task<Row> GetRowAsync(int id);
        Task<List<Row>> GetRowListAsync();
        Task<List<Row>> GetRowListAsync(List<int> rowIds);
    }
}
