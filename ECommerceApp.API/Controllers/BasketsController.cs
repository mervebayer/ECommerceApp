using ECommerceApp.API.Services;
using ECommerceApp.Application.DTOs.Baskets;
using ECommerceApp.Application.Interfaces;
using ECommerceApp.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceApp.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BasketsController : ControllerBase
    {
        private readonly IBasketService _basketService;
        private readonly IBasketIdentityService _basketIdentityService;

        public BasketsController(IBasketService basketService, IBasketIdentityService basketIdentityService)
        {
            _basketService = basketService;
            _basketIdentityService = basketIdentityService;
        }

        [HttpGet]
        public async Task<ActionResult<CustomerBasket>> GetBasketById(CancellationToken cancellationToken)
        {
            var basketId = await _basketIdentityService.GetOrCreateBasketIdAsync(cancellationToken);
            var basket = await _basketService.GetOrCreateBasketAsync(basketId);
            return Ok(basket);
        }

        [HttpPost("items")]
        public async Task<ActionResult<CustomerBasket>> AddOrIncrementItem([FromBody] BasketItemUpsertDto dto, CancellationToken cancellationToken)
        {
            var basketId = await _basketIdentityService.GetOrCreateBasketIdAsync(cancellationToken);
            var basket = await _basketService.AddItemAsync(basketId, dto.ProductId, dto.Quantity);
            return Ok(basket);
        }

        [HttpPost("items/{productId:long}/decrease")]
        public async Task<IActionResult> DecreaseItem(long productId, [FromQuery] int quantity = 1, CancellationToken cancellationToken = default)
        {
            var basketId = await _basketIdentityService.GetOrCreateBasketIdAsync(cancellationToken);
            await _basketService.DecreaseItemAsync(basketId, productId, quantity);
            return NoContent();
        }

        [HttpDelete("items/{productId:long}")]
        public async Task<IActionResult> RemoveItem(long productId, CancellationToken cancellationToken)
        {
            var basketId = await _basketIdentityService.GetOrCreateBasketIdAsync(cancellationToken);
            await _basketService.RemoveItemAsync(basketId, productId);
            return NoContent();
        }
    }
}
