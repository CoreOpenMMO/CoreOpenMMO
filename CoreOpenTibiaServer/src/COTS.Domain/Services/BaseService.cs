using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace COTS.Domain.Services
{
    using Domain.Interfaces.Repositories;
    using Domain.Interfaces.Services;

    public class BaseService<TEntity> : IDisposable, IServiceBase<TEntity> where TEntity : class
    {
        private readonly IRepositoryBase<TEntity> _repository;

        public BaseService(IRepositoryBase<TEntity> repository) => 
            _repository = repository;

        public virtual Task<bool> Add(TEntity obj) => 
            _repository.Add(obj);

        public Task<IEnumerable<TEntity>> GetAll() =>
            _repository.GetAll();

        public virtual Task<TEntity> GetById(int id) =>
            _repository.GetById(id);

        public Task<TEntity> GetByGuid(Guid id) =>
            _repository.GetByGuid(id);

        public virtual void Remove(TEntity obj) => 
            _repository.Remove(obj);

        public virtual void Update(TEntity obj) => 
            _repository.Update(obj);

        public void Dispose() => 
            _repository.Dispose();
    }
}
