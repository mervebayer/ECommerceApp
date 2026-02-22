using ECommerceApp.Core.Interfaces.Repositories;
using ECommerceApp.Core.Models;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ECommerceApp.Data.Repositories
{
    public class BasketRepository : IBasketRepository
    {
        private readonly IDatabase _database;

        public BasketRepository(IConnectionMultiplexer redis)
        {
            _database = redis.GetDatabase();
        }

        public async Task<CustomerBasket?> GetBasketAsync(string basketId)
        {
            var data = await _database.StringGetAsync(GetBasketKey(basketId));
            if (data.IsNullOrEmpty) return null;
            return JsonSerializer.Deserialize<CustomerBasket>(data!);
        }

        public async Task<CustomerBasket?> UpdateBasketAsync(CustomerBasket basket)
        {
            var data = JsonSerializer.Serialize(basket);
            var created = await _database.StringSetAsync(GetBasketKey(basket.Id), data, TimeSpan.FromDays(30));
            if (!created) return null;
            return await GetBasketAsync(basket.Id);
        }

        public async Task<bool> DeleteBasketAsync(string basketId)
        {
            return await _database.KeyDeleteAsync(GetBasketKey(basketId));
        }
        private static string GetBasketKey(string basketId) => $"basket:{basketId}";
    }
}
