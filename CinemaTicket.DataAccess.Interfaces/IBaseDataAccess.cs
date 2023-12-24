using System.Collections.Generic;
using System.Threading.Tasks;
using CinemaTicket.Entities;

namespace CinemaTicket.DataAccess.Interfaces
{
    public interface IBaseDataAccess
    {
        Task CreateAsync(IDbEntity entity);
        Task CreateListAsync<T>(IEnumerable<T> entities) where T : class, IDbEntity;
        void Update(IDbEntity entity);
        void UpdateList<T>(IEnumerable<T> entities) where T : class, IDbEntity;
        void Delete(IDbEntity entity);
        void DeleteList<T>(IEnumerable<T> entities) where T : class, IDbEntity;
        Task CommitAsync();
        void Dispose();
    }
}
