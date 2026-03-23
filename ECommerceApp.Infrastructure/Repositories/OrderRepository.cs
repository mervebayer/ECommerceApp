using ECommerceApp.Domain.Entities;
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

    }
}
