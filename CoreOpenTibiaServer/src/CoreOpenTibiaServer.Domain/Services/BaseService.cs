using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using COTS.Domain.Interfaces.Repositories;
using COTS.Domain.Interfaces.Services;

namespace COTS.Domain.Services
{
    public class BaseService<TEntity> : IDisposable, IBaseService<TEntity> where TEntity : class
    {
        private readonly IRepositoryBase<TEntity> _repository;

        public BaseService(IRepositoryBase<TEntity> repository)
        {
            _repository = repository;
        }

        public virtual Task<bool> Add(TEntity obj)
        {
            return _repository.Add(obj);
        }

        public Task<IEnumerable<TEntity>> GetAll()
        {
            return _repository.GetAll();
        }
        
        public virtual Task<TEntity> GetById(int id)
        {
            return _repository.GetById(id);
        }

        public Task<TEntity> GetByGuid(Guid id)
        {
            return _repository.GetByGuid(id);
        }

        public virtual void Remove(TEntity obj)
        {
            _repository.Remove(obj);
        }
        
        public virtual void Update(TEntity obj)
        {
            _repository.Update(obj);
        }
        public void Dispose()
        {
            _repository.Dispose();
        }
    }
}
