using ECommerceApp.Application.Interfaces;
using ECommerceApp.Domain.Entities;
using ECommerceApp.Domain.Exceptions;
using ECommerceApp.Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApp.Application.Services
{
    public class BasketService : IBasketService
    {
        private readonly IBasketRepository _basketRepository;
        private readonly IProductRepository _productRepository;

        public BasketService(IBasketRepository basketRepository, IProductRepository productRepository)
        {
            _basketRepository = basketRepository;
            _productRepository = productRepository;
        }
        public async Task<CustomerBasket> GetOrCreateBasketAsync(string basketId)
        {
            var basket = await _basketRepository.GetBasketAsync(basketId);
            return basket ?? new CustomerBasket(basketId);
        }

        public async Task MergeBasketsAsync(string sourceBasketId, string targetBasketId)
        {
            if (string.IsNullOrWhiteSpace(sourceBasketId) || string.IsNullOrWhiteSpace(targetBasketId))
                return;

            if (sourceBasketId == targetBasketId)
                return;

            var sourceBasket = await _basketRepository.GetBasketAsync(sourceBasketId);
            if (sourceBasket == null || !sourceBasket.Items.Any())
                return;

            var targetBasket = await _basketRepository.GetBasketAsync(targetBasketId) ?? new CustomerBasket(targetBasketId);

            foreach (var sourceItem in sourceBasket.Items)
            {
                var targetItem = targetBasket.Items.FirstOrDefault(x => x.ProductId == sourceItem.ProductId);

                if (targetItem == null)
                {
                    targetBasket.Items.Add(sourceItem);
                    continue;
                }

                targetItem.Quantity += sourceItem.Quantity;
                targetItem.ProductName = sourceItem.ProductName;
                targetItem.Price = sourceItem.Price;
                targetItem.ImageUrl = sourceItem.ImageUrl;
            }

            await _basketRepository.UpdateBasketAsync(targetBasket);
            await _basketRepository.DeleteBasketAsync(sourceBasketId);
        }



        public async Task<CustomerBasket> AddItemAsync(string basketId, long productId, int quantity)
        {
            if (quantity <= 0) throw new ArgumentOutOfRangeException(nameof(quantity));

            var basket = await _basketRepository.GetBasketAsync(basketId)
                         ?? new CustomerBasket(basketId);

            var product = await _productRepository.GetByIdWithImagesAsync(productId)
                ?? throw new NotFoundException("Product not found.");

            var mainImageUrl = product.Images?.FirstOrDefault(x => x.IsMain)?.Url;

            var existingItem = basket.Items.FirstOrDefault(x => x.ProductId == productId);

            if (existingItem == null)
            {
                existingItem = new BasketItem { ProductId = product.Id, Quantity = 0 };
                basket.Items.Add(existingItem);
            }
            existingItem.Quantity += quantity;
            existingItem.ProductName = product.Name;
            existingItem.Price = product.Price;
            existingItem.ImageUrl = mainImageUrl;
            return await _basketRepository.UpdateBasketAsync(basket) ?? basket;
        }
       
    

        // TODO: Potential lost-update under concurrent requests (GET-modify-SET). Add Redis Hash or Lua script for atomic updates in production.
        public async Task<CustomerBasket?> UpdateBasketAsync(CustomerBasket basket)
        {
            var existingBasket = await GetOrCreateBasketAsync(basket.Id);

            foreach (var item in basket.Items)
            {
                if (item.Quantity <= 0)
                    throw new ArgumentException("Quantity must be greater than zero.");

                var existingItem = existingBasket.Items.FirstOrDefault(x => x.ProductId == item.ProductId);

                if (item.Quantity == 0)
                {
                    if (existingItem != null)
                        existingBasket.Items.Remove(existingItem);
                    continue;
                }
                if (existingItem != null)
                    existingItem.Quantity += item.Quantity;
                else
                    existingBasket.Items.Add(item);
            }

            return await _basketRepository.UpdateBasketAsync(existingBasket);
        }
        //TODO: Delete 
        public async Task RemoveOrDecreaseItemAsync(string basketId, long productId, int quantity = 1)
        {
            if (quantity <= 0) throw new ArgumentOutOfRangeException(nameof(quantity));

            var basket = await GetOrCreateBasketAsync(basketId);

            var item = basket.Items.FirstOrDefault(x => x.ProductId == productId);
            if (item == null) return;

            item.Quantity -= quantity;

            if (item.Quantity <= 0)
                basket.Items.Remove(item);

            await _basketRepository.UpdateBasketAsync(basket);
        }

        public async Task RemoveItemAsync(string basketId, long productId)
        {
            var basket = await GetOrCreateBasketAsync(basketId);

            var item = basket.Items.FirstOrDefault(x => x.ProductId == productId);
            if (item == null) return;

            basket.Items.Remove(item);

            await _basketRepository.UpdateBasketAsync(basket);
        }

        public async Task DecreaseItemAsync(string basketId, long productId, int quantity = 1)
        {
            if (quantity <= 0) throw new ArgumentOutOfRangeException(nameof(quantity));

            var basket = await GetOrCreateBasketAsync(basketId);

            var item = basket.Items.FirstOrDefault(x => x.ProductId == productId);
            if (item == null) return;

            item.Quantity -= quantity;

            if (item.Quantity <= 0)
                basket.Items.Remove(item);

            await _basketRepository.UpdateBasketAsync(basket);
        }

    }
}
