using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using CinemaTicket.DataAccess.Interfaces;
using CinemaTicket.Entities;


namespace CinemaTicket.DataAccess
{
    public class BaseDataAccess : IBaseDataAccess, IDisposable
    {
        protected readonly CinemaManagerContext cinemaManagerContext;
        private IDbContextTransaction transaction;
        public BaseDataAccess(CinemaManagerContext cinemaManagerContext)
        {
            this.cinemaManagerContext = cinemaManagerContext;
        }
        public async Task CreateAsync (IDbEntity entity)
        {
            await cinemaManagerContext.AddAsync(entity);
        }
        public async Task CreateListAsync<T>(IEnumerable<T> entities) where T : class, IDbEntity
        {
            await cinemaManagerContext.AddRangeAsync(entities);
        }
        public void Update(IDbEntity entity)
        {
            cinemaManagerContext.Entry(entity).State = EntityState.Modified;
        }
        public void UpdateList<T>(IEnumerable<T> entities) where T : class, IDbEntity
        {
            foreach (var entity in entities)
            {
                //TODO поле ModifiedOn здесь недоступно, изменять это поле нужно заранее в сервисах
                cinemaManagerContext.Entry(entity).State = EntityState.Modified;
            }
        }
        public void Delete(IDbEntity entity)
        {
            cinemaManagerContext.Entry(entity).State = EntityState.Deleted;
            cinemaManagerContext.Remove(entity);
        }
        public void DeleteList<T>(IEnumerable<T> entities) where T : class, IDbEntity
        {
            foreach (var entity in entities)
            {
                cinemaManagerContext.Entry(entity).State = EntityState.Deleted;
            }
        }
        public async Task CommitAsync()
        {
            await cinemaManagerContext.SaveChangesAsync();
            if (transaction != null)
            {
                await transaction.CommitAsync();
            }
        }
        public void Dispose()
        {
            if (cinemaManagerContext != null)
            {
                cinemaManagerContext.Dispose();
            }
            if (transaction != null)
            {
                transaction.Dispose();
            }
        }
    }
}
