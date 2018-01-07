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
        protected readonly COTSContext Db;
        protected readonly DbSet<TEntity> DbSet;

        public BaseRepository(COTSContext context)
        {
            Db = context;
            DbSet = Db.Set<TEntity>();
        }

        public virtual async Task<bool> Add(TEntity obj) => 
            await DbSet.AddAsync(obj) != null;

        public virtual async Task<IEnumerable<TEntity>> GetAll() => 
            await Db.Set<TEntity>().ToListAsync();

        public virtual async Task<TEntity> GetById(int id) => 
            await Db.Set<TEntity>().FindAsync(id);

        public virtual async Task<TEntity> GetByGuid(Guid guid) => 
            await Db.Set<TEntity>().FindAsync(guid);

        public virtual async Task<TEntity> GetById(Guid id) => 
            await Db.Set<TEntity>().FindAsync(id);

        public virtual void Remove(TEntity obj) => 
            Db.Set<TEntity>().Remove(obj);

        public virtual void Update(TEntity obj) =>
            Db.Entry(obj).State = EntityState.Modified;

        public virtual IQueryable<TEntity> FindBy(Expression<Func<TEntity, bool>> predicate) => 
            Db.Set<TEntity>().Where(predicate);

        public void Dispose()
        {
            Db.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
