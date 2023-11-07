using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CinemaTicket.Entities;

namespace CinemaTicket.DataAccess.Interfaces
{
    public interface IRowDataAccess
    {
        Task CreateAsync(Row row);
        Task CreateAsync(List<Row> rows);
        Task<Row> GetRowAsync(int id);
        Task<List<Row>> GetRowListAsync();
        Task<List<Row>> GetRowListAsync(List<int> rowIds);
        Task UpdateRowAsync(Row row);
        Task DeleteRowAsync(Row row);
    }
}
