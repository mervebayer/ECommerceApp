using ECommerceApp.Application.DTOs.Orders;
using ECommerceApp.Application.DTOs.QueryParams;
using ECommerceApp.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ECommerceApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<CreateOrderResponseDto>> CreateOrderAsync([FromBody] CreateOrderRequestDto request, CancellationToken cancellationToken)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var basketId = Request.Cookies["basketId"];

            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized();

            if (string.IsNullOrWhiteSpace(basketId))
                return BadRequest("Basket not found.");

            var result = await _orderService.CreateOrderAsync(userId, basketId, request, cancellationToken);

            return Ok(result);
        }


        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<OrderListDto>>> GetMyOrdersAsync([FromQuery] OrderQueryParams queryParams, CancellationToken cancellationToken)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized();

            var result = await _orderService.GetMyOrdersAsync(userId, queryParams, cancellationToken);

            return Ok(result);
        }

        [Authorize]
        [HttpGet("{orderId:long}")]
        public async Task<ActionResult<OrderDetailDto>> GetOrderDetailAsync(long orderId, CancellationToken cancellationToken)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized();

            var result = await _orderService.GetOrderByIdAndUserIdAsync(userId, orderId, cancellationToken);

            return Ok(result);
        }

        [Authorize]
        [HttpPost("{orderId:long}/cancel")]
        public async Task<IActionResult> CancelOrderAsync(long orderId, CancellationToken cancellationToken)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized();

            await _orderService.CancelOrderAsync(userId, orderId, cancellationToken);

            return NoContent();
        }

        [Authorize(Roles = "Admin,StoreManager")]
        [HttpPatch("{orderId:long}/status")]
        public async Task<IActionResult> UpdateOrderStatus(long orderId, [FromBody] UpdateOrderStatusRequestDto request, CancellationToken cancellationToken)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized();

            await _orderService.UpdateOrderStatusAsync(
                userId,
                orderId,
                request.Status,
                cancellationToken);

            return NoContent();
        }
    }
}
