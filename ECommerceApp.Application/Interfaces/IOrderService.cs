using ECommerceApp.Application.DTOs.Orders;
using ECommerceApp.Application.DTOs.QueryParams;
using ECommerceApp.Domain.Entities;
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
        Task<CreateOrderResponseDto> CreateOrderAsync(string userId, string basketId, CreateOrderRequestDto request, CancellationToken cancellationToken);
    }
}
