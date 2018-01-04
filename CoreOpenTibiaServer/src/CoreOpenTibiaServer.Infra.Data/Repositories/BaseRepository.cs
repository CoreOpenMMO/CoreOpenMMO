using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using COTS.Data.Context;
using COTS.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace COTS.Data.Repositories
{
    public class BaseRepository<TEntity> : IDisposable, IRepositoryBase<TEntity> where TEntity : class
    {
        protected COTSContext Db;

        protected DbSet<TEntity> DbSet;

        public BaseRepository(COTSContext context)
        {
            Db = context;
            DbSet = Db.Set<TEntity>();
        }
        
        public async Task<bool> Add(TEntity obj)
        {
            return await DbSet.AddAsync(obj) != null;
        }
        
        public async Task<IEnumerable<TEntity>> GetAll()
        {
            return await Db.Set<TEntity>().ToListAsync();
        }
        
        public virtual async Task<TEntity> GetById(int id)
        {
            return await Db.Set<TEntity>().FindAsync(id);
        }

        public virtual async Task<TEntity> GetByGuid(Guid guid)
        {
            return await Db.Set<TEntity>().FindAsync(guid);
        }

        public virtual async Task<TEntity> GetById(Guid id)
        {
            return await Db.Set<TEntity>().FindAsync(id);
        }

        public virtual void Remove(TEntity obj)
        {
            Db.Set<TEntity>().Remove(obj);
        }

        public virtual void Update(TEntity obj)
        {
            Db.Entry(obj).State = EntityState.Modified;
        }
        public virtual IQueryable<TEntity> FindBy(Expression<Func<TEntity, bool>> predicate)
        {
            return Db.Set<TEntity>().Where(predicate);
        }
        public void Dispose()
        {
            Db.Dispose();
            GC.SuppressFinalize(this);
        }

    }
}
