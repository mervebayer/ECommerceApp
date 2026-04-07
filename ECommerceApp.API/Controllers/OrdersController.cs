using ECommerceApp.Application.DTOs.Orders;
using ECommerceApp.Application.DTOs.Orders.Admin;
using ECommerceApp.Application.DTOs.Payments;
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

            return Ok(new
            {
                message = "Your order has been created successfully.",
                data = result
            });
        }


        [Authorize]
        [HttpPost("{orderId:long}/pay")]
        public async Task<ActionResult<PayOrderResponseDto>> PayOrderAsync(long orderId, [FromBody] PayOrderRequestDto request, CancellationToken cancellationToken)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized();

            var buyerIp = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1";
            var result = await _orderService.PayOrderAsync(userId, orderId, buyerIp, request, cancellationToken);

            if (!result.IsPaymentSuccessful)
            {
                return BadRequest(new
                {
                    message = result.PaymentErrorMessage ?? "Payment could not be completed.",
                    data = result
                });
            }

            return Ok(new
            {
                message = "Your payment has been completed successfully.",
                data = result
            });
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

            await _orderService.UpdateOrderStatusAsync(orderId, request.Status, cancellationToken);

            return NoContent();
        }

        [Authorize(Roles = "Admin,StoreManager")]
        [HttpGet("admin")]
        public async Task<ActionResult<IReadOnlyList<AdminOrderListDto>>> GetAllOrdersAsync([FromQuery] AdminOrderQueryParams queryParams, CancellationToken cancellationToken)
        {
            var result = await _orderService.GetAllOrdersAsync(queryParams, cancellationToken);

            return Ok(result);
        }

        [Authorize(Roles = "Admin,StoreManager")]
        [HttpPost("{orderId:long}/admin-cancel")]
        public async Task<IActionResult> CancelOrderAsAdminAsync(long orderId, CancellationToken cancellationToken)
        {
            await _orderService.CancelOrderByAdminAsync(orderId, cancellationToken);
            return NoContent();
        }

        [Authorize(Roles = "Admin,StoreManager")]
        [HttpGet("admin/{orderId:long}")]
        public async Task<ActionResult<AdminOrderDetailDto>> GetAdminOrderDetailAsync(long orderId, CancellationToken cancellationToken)
        {

            var result = await _orderService.GetOrderById(orderId, cancellationToken);

            return Ok(result);
        }


    }
}
