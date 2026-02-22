using ECommerceApp.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApp.Core.Interfaces.Services
{
    public interface IBasketService
    {
        Task<CustomerBasket> GetOrCreateBasketAsync(string basketId);
        Task<CustomerBasket?> UpdateBasketAsync(CustomerBasket basket);
        //Task RemoveOrDecreaseItemAsync(string basketId, long productId, int quantity = 1);
        Task<CustomerBasket> AddItemAsync(string basketId, long productId, int quantity);
        Task RemoveItemAsync(string basketId, long productId);
        Task DecreaseItemAsync(string basketId, long productId, int quantity = 1);
    }
}
