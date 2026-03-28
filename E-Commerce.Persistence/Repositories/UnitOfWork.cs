using E_Commerce.Domain.Contracts;
using E_Commerce.Domain.Entities;
using E_Commerce.Persistence.Data.DbContexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Persistence.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly StoreDbContext _dbContext;
        private readonly Dictionary<Type, object> _repositories = [];

        public UnitOfWork(StoreDbContext dbContext)
        {
            this._dbContext = dbContext;
        }
        public IGenaricRrepository<TEntity, TKey> GetRrepository<TEntity, TKey>() where TEntity : BaseEntity<TKey>
        {
            var EntityType = typeof(TEntity);

            if(_repositories.TryGetValue(EntityType, out object? repository))
            {
                return (IGenaricRrepository<TEntity, TKey>)repository;
            }
            var newRepo = new GenaricRepository<TEntity, TKey>(this._dbContext);
            _repositories[EntityType] =newRepo;
            return newRepo;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _dbContext.SaveChangesAsync();
        }
    }
}
