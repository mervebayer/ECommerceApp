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

        public BasketsController(IBasketService basketService)
        {
            _basketService = basketService;
        }
        [HttpGet]
        public async Task<ActionResult<CustomerBasket>> GetBasketById()
        {
            var basketId = GetOrCreateBasketId();
            var basket = await _basketService.GetOrCreateBasketAsync(basketId);
            return Ok(basket);
        }
        [HttpPost("items")]
        public async Task<ActionResult<CustomerBasket>> AddOrIncrementItem([FromBody] BasketItemUpsertDto dto)
        {
            var basketId = GetOrCreateBasketId();
            var basket = await _basketService.AddItemAsync(basketId, dto.ProductId, dto.Quantity);
            return Ok(basket);
        }

        //[HttpPost]
        //public async Task<ActionResult<CustomerBasket>> UpdateBasket(CustomerBasket basket)
        //{
        //    var basketId = GetOrCreateBasketId();
        //    basket.Id = basketId;
        //    var updatedBasket = await _basketService.UpdateBasketAsync(basket);

        //    if (updatedBasket == null)
        //        return BadRequest("An error occurred while updating the basket.");

        //    return Ok(updatedBasket);
        //}

        [HttpPost("items/{productId:long}/decrease")]
        public async Task<IActionResult> DecreaseItem(long productId, [FromQuery] int quantity = 1)
        {
            var basketId = GetOrCreateBasketId();
            await _basketService.DecreaseItemAsync(basketId, productId, quantity);
            return NoContent();
        }

        [HttpDelete("items/{productId:long}")]
        public async Task<IActionResult> RemoveItem(long productId)
        {
            var basketId = GetOrCreateBasketId();
            await _basketService.RemoveItemAsync(basketId, productId);
            return NoContent();
        }
        private string GetOrCreateBasketId()
        {
            var basketId = Request.Cookies["basketId"];
            if (string.IsNullOrEmpty(basketId))
            {
                basketId = Guid.NewGuid().ToString();
                var cookieOptions = new CookieOptions
                {
                    Expires = DateTime.Now.AddDays(30),
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Lax
                };
                Response.Cookies.Append("basketId", basketId, cookieOptions);
            }
            return basketId;
        }
    }
}
