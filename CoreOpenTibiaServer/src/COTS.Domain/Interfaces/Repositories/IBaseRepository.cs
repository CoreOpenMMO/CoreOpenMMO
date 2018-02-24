using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace COTS.Domain.Interfaces.Repositories
{
    public interface IRepositoryBase<TEntity> where TEntity : class
    {
        Task<bool> Add(TEntity obj);
        Task<IEnumerable<TEntity>> GetAll();
        Task<TEntity> GetById(int id);
        Task<TEntity> GetByGuid(Guid guid);
        void Update(TEntity obj);
        void Remove(TEntity obj);
        IQueryable<TEntity> FindBy(Expression<Func<TEntity, bool>> predicate);
        void Dispose();
    }
}
