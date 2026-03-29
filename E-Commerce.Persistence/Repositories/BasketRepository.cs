using E_Commerce.Domain.Contracts;
using E_Commerce.Domain.Entities.BasketModule;
using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace E_Commerce.Persistence.Repositories
{
    public class BasketRepository : IBasketRepository
    {
        private readonly IDatabase _database;
        public BasketRepository(IConnectionMultiplexer connection)
        {
            _database =connection.GetDatabase();
        }
        public async Task<bool> DeleteBasketAsync(string Id)
        {
            return await _database.KeyDeleteAsync(Id);
        }

        public async Task<CustomerBasket?> GetBasketAsync(string Id)
        {
            var Basket = await _database.StringGetAsync(Id);
            if (Basket.IsNullOrEmpty)
                return null;
            else
                return JsonSerializer.Deserialize<CustomerBasket>(Basket!);
        }

        public async Task<CustomerBasket?> CreateOrUpdateBasketAsync(CustomerBasket basket, TimeSpan timeToLive = default)
        {
            var JsonBasket = JsonSerializer.Serialize(basket);
            bool IsCreatedOrUpdated = await _database.StringSetAsync(basket.Id ,JsonBasket ,(timeToLive==default)?TimeSpan.FromDays(7) : timeToLive);
            if (IsCreatedOrUpdated)
            {
                return await GetBasketAsync(basket.Id);
            }
            else
            {
                return null;
            }
        }
    }
}
