using E_Commerce.Domain.Contracts;
using E_Commerce.Domain.Entities;
using E_Commerce.Persistence.Data.DbContexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Persistence.Repositories
{
    public class GenaricRepository<TEntity, TKey> : IGenaricRrepository<TEntity, TKey> where TEntity : BaseEntity<TKey>
    {
        private readonly StoreDbContext _storeDbContext;

        public GenaricRepository(StoreDbContext storeDbContext) 
        {
            this._storeDbContext = storeDbContext;
        }
        public async Task AddAsync(TEntity entity)
        {
            await _storeDbContext.Set<TEntity>().AddAsync(entity);
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync()
            //(Expression<Func<TEntity,bool>>? condtion =default,
            //List<Expression<Func<TEntity ,object>>>? Includes =default)
        {
            //if(condtion is not null)
            //{
            //    return await _storeDbContext.Set<TEntity>().Where(condtion).ToListAsync();
            //}
            //if (Includes is not null)
            //{
            //    IQueryable<TEntity> EntryPoint = _storeDbContext.Set<TEntity>();
            //    foreach (var includeExp in Includes)
            //    {
            //        EntryPoint = EntryPoint.Include(includeExp);
            //    }
            //    return await EntryPoint.ToListAsync();
            //}
            return await _storeDbContext.Set<TEntity>().ToListAsync();
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync(ISpecifications<TEntity, TKey> specifications)
        {
            IQueryable<TEntity> Query = _storeDbContext.Set<TEntity>();
            Query = SpecificationsEvaluator.CreateQuery(Query, specifications);
            return await Query.ToListAsync();
        }

        public async Task<TEntity?> GetByIdAsync(TKey id)
        {
            return await _storeDbContext.Set<TEntity>().FindAsync(id);
        }

        public void Remove(TEntity entity)
        {
            _storeDbContext.Set<TEntity>().Remove(entity);
        }

        public void Update(TEntity entity)
        {
            _storeDbContext.Set<TEntity>().Update(entity);
        }
    }
}
