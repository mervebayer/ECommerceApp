using ECommerceApp.Application.DTOs.Orders;
using ECommerceApp.Application.DTOs.Orders.Admin;
using ECommerceApp.Application.DTOs.QueryParams;
using ECommerceApp.Domain.Entities;
using ECommerceApp.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApp.Application.Interfaces
{
    public interface IOrderService 
    {
        Task<PagedResult<OrderListDto>> GetMyOrdersAsync(string userId, OrderQueryParams queryParams, CancellationToken cancellationToken);
        Task<CreateOrderResponseDto> CreateOrderAsync(string userId, string basketId, string buyerIp, CreateOrderRequestDto request, CancellationToken cancellationToken);
        Task<OrderDetailDto> GetOrderByIdAndUserIdAsync(string userId, long orderId, CancellationToken cancellationToken);
        Task CancelOrderAsync(string userId, long orderId, CancellationToken cancellationToken);
        Task UpdateOrderStatusAsync(long orderId, OrderStatus newStatus, CancellationToken cancellationToken);
        Task CancelOrderByAdminAsync(long orderId, CancellationToken cancellationToken);
        Task<PagedResult<AdminOrderListDto>> GetAllOrdersAsync(AdminOrderQueryParams queryParams, CancellationToken cancellationToken);

        Task<AdminOrderDetailDto> GetOrderById(long orderId, CancellationToken cancellationToken);
    }
}
