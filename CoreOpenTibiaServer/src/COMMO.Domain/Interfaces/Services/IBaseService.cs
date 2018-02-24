using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace COMMO.Domain.Interfaces.Services
{
    public interface IServiceBase<TEntity> where TEntity : class
    {
        Task<bool> Add(TEntity obj);
        Task<IEnumerable<TEntity>> GetAll();
        Task<TEntity> GetById(int id);
        Task<TEntity> GetByGuid(Guid guid);
        void Update(TEntity obj);
        void Remove(TEntity obj);
        void Dispose();
    }
}
