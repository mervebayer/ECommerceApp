using ECommerceApp.Domain.Entities;
using ECommerceApp.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApp.Domain.Interfaces.Repositories
{
    public interface IOrderRepository : IGenericRepository<Order>
    {
        Task<IReadOnlyList<Order>> GetMyOrdersAsync(string userId, int pageNumber, int pageSize, CancellationToken cancellationToken);
        Task<int> CountAsync(string userId, CancellationToken cancellationToken);
        Task<Order?> GetByIdAndUserIdAsync(string userId, long orderId, CancellationToken cancellationToken);
        Task<Dictionary<long, int>> GetReservedQuantitiesAsync(IEnumerable<long> productIds, CancellationToken cancellationToken = default);
        Task<List<Order>> GetExpiredPendingPaymentOrdersAsync(CancellationToken cancellationToken = default);
        Task<Dictionary<long, int>> GetReservedQuantitiesExcludingOrderAsync(IEnumerable<long> productIds, long excludedOrderId, CancellationToken cancellationToken = default);
        Task<Order?> GetOrderByIdWithItemsAsync(long orderId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Order>> GetAllOrdersAsync(int pageNumber, int pageSize, OrderStatus? status, string? orderNumber, string? userId, DateTime? startDate, DateTime? endDate, CancellationToken cancellationToken = default);
        Task<int> CountAllAsync(OrderStatus? status, string? orderNumber, string? userId, DateTime? startDate, DateTime? endDate, CancellationToken cancellationToken = default);

        Task<Order?> GetOrderByIdAsync(long orderId, CancellationToken cancellationToken);

    }
}
