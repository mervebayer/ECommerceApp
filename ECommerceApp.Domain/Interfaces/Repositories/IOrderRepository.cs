using ECommerceApp.Domain.Entities;
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
    }
}
