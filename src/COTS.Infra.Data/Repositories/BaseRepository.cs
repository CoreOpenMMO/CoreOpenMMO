using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace COTS.Data.Repositories
{
    using Context;
    using Domain.Interfaces.Repositories;

    public class BaseRepository<TEntity> : IDisposable, IRepositoryBase<TEntity> where TEntity : class
    {
        protected readonly COTSContext _db;
        protected readonly DbSet<TEntity> _dbSet;

        public BaseRepository(COTSContext context)
        {
            _db = context;
            _dbSet = _db.Set<TEntity>();
        }

        public virtual async Task<bool> Add(TEntity obj) => 
            await _dbSet.AddAsync(obj) != null;

        public virtual async Task<IEnumerable<TEntity>> GetAll() => 
            await _db.Set<TEntity>().ToListAsync();

        public virtual async Task<TEntity> GetById(int id) => 
            await _db.Set<TEntity>().FindAsync(id);

        public virtual async Task<TEntity> GetByGuid(Guid guid) => 
            await _db.Set<TEntity>().FindAsync(guid);

        public virtual async Task<TEntity> GetById(Guid id) => 
            await _db.Set<TEntity>().FindAsync(id);

        public virtual void Remove(TEntity obj) => 
            _db.Set<TEntity>().Remove(obj);

        public virtual void Update(TEntity obj) =>
            _db.Entry(obj).State = EntityState.Modified;

        public virtual IQueryable<TEntity> FindBy(Expression<Func<TEntity, bool>> predicate) => 
            _db.Set<TEntity>().Where(predicate);

        public void Dispose()
        {
            _db.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
