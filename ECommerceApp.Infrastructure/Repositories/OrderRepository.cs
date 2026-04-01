using ECommerceApp.Application.DTOs.QueryParams;
using ECommerceApp.Domain.Entities;
using ECommerceApp.Domain.Enums;
using ECommerceApp.Domain.Interfaces.Repositories;
using ECommerceApp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApp.Infrastructure.Repositories
{
    public class OrderRepository : GenericRepository<Order>, IOrderRepository
    {
        private readonly AppDbContext _context;

        public OrderRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }


        #region admin
        public async Task<IReadOnlyList<Order>> GetAllOrdersAsync(int pageNumber, int pageSize, OrderStatus? status, string? orderNumber, string? userId, DateTime? startDate, DateTime? endDate, CancellationToken cancellationToken = default)
        {
            var query = _context.Orders.AsNoTracking().Include(x => x.Items).AsQueryable();

            if (status.HasValue)
                query = query.Where(x => x.Status == status.Value);
            

            if (!string.IsNullOrWhiteSpace(orderNumber))           
                query = query.Where(x => x.OrderNumber.Contains(orderNumber));
            
            if (!string.IsNullOrWhiteSpace(userId))            
                query = query.Where(x => x.UserId == userId);           

            if (startDate.HasValue)
            {
                var start = startDate.Value.Date;
                query = query.Where(x => x.CreatedDate >= start);
            }

            if (endDate.HasValue)
            {
                var endExclusive = endDate.Value.Date.AddDays(1);
                query = query.Where(x => x.CreatedDate < endExclusive);
            }

            return await query.OrderByDescending(x => x.CreatedDate).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);
        }



        public async Task<int> CountAllAsync(OrderStatus? status, string? orderNumber, string? userId, DateTime? startDate, DateTime? endDate, CancellationToken cancellationToken = default)
        {
            var query = _context.Orders.AsNoTracking() .AsQueryable();

            if (status.HasValue)            
                query = query.Where(x => x.Status == status.Value);           

            if (!string.IsNullOrWhiteSpace(orderNumber))            
                query = query.Where(x => x.OrderNumber.Contains(orderNumber));
            
            if (!string.IsNullOrWhiteSpace(userId))            
                query = query.Where(x => x.UserId == userId);
            
            if (startDate.HasValue)
            {
                var start = startDate.Value.Date;
                query = query.Where(x => x.CreatedDate >= start);
            }

            if (endDate.HasValue)
            {
                var endExclusive = endDate.Value.Date.AddDays(1);
                query = query.Where(x => x.CreatedDate < endExclusive);
            }

            return await query.CountAsync(cancellationToken);
        }

        public async Task<Order?> GetOrderByIdAsync(long orderId, CancellationToken cancellationToken)
        {
            return await _context.Orders.AsNoTracking().Include(x => x.Items).SingleOrDefaultAsync(x => x.Id == orderId, cancellationToken);
        }

     



        #endregion


        public async Task<IReadOnlyList<Order>> GetMyOrdersAsync(string userId,int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            return await _context.Orders.AsNoTracking().Where(x => x.UserId == userId)
                .Include(x => x.Items)
                .OrderByDescending(x => x.CreatedDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);
        }

        public async Task<int> CountAsync(string userId,CancellationToken cancellationToken)
        {
            return await _context.Orders.AsNoTracking()
                .CountAsync(x => x.UserId == userId, cancellationToken);
        }

        public async Task<Order?> GetByIdAndUserIdAsync(string userId, long orderId, CancellationToken cancellationToken)
        {
            return await _context.Orders.Include(x => x.Items).SingleOrDefaultAsync(x => x.Id == orderId && x.UserId == userId, cancellationToken);
        }

        public async Task<Dictionary<long, int>> GetReservedQuantitiesAsync(IEnumerable<long> productIds, CancellationToken cancellationToken = default)
        {
            var ids = productIds.Distinct().ToList();
            var now = DateTime.UtcNow;

            return await _context.OrderItems
                .Where(oi => ids.Contains(oi.ProductId) && oi.Order.Status == OrderStatus.PendingPayment && oi.Order.ReservationExpiresAt > now)
                .GroupBy(oi => oi.ProductId)
                .Select(g => new
                {
                    ProductId = g.Key,
                    TotalReserved = g.Sum(x => x.Quantity)
                })
                .ToDictionaryAsync(x => x.ProductId, x => x.TotalReserved, cancellationToken);
        }

        public async Task<List<Order>> GetExpiredPendingPaymentOrdersAsync(CancellationToken cancellationToken = default)
        {
            var now = DateTime.UtcNow;

            return await _context.Orders
                .Where(x => x.Status == OrderStatus.PendingPayment && x.ReservationExpiresAt <= now)
                .ToListAsync(cancellationToken);
        }

        public async Task<Dictionary<long, int>> GetReservedQuantitiesExcludingOrderAsync(IEnumerable<long> productIds,long excludedOrderId, CancellationToken cancellationToken = default)
        {
            var ids = productIds.Distinct().ToList();
            var now = DateTime.UtcNow;

            return await _context.OrderItems
                .Where(oi =>
                    ids.Contains(oi.ProductId) &&
                    oi.Order.Status == OrderStatus.PendingPayment &&
                    oi.Order.ReservationExpiresAt > now &&
                    oi.OrderId != excludedOrderId)
                .GroupBy(oi => oi.ProductId)
                .Select(g => new
                {
                    ProductId = g.Key,
                    TotalReserved = g.Sum(x => x.Quantity)
                })
                .ToDictionaryAsync(x => x.ProductId, x => x.TotalReserved, cancellationToken);
        }

        public async Task<Order?> GetOrderByIdWithItemsAsync(long orderId, CancellationToken cancellationToken = default)
        {
            return await _context.Orders
                .Include(x => x.Items)
                .SingleOrDefaultAsync(x => x.Id == orderId, cancellationToken);
        }
    }
}
