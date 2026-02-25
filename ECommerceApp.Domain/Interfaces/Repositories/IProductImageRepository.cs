using ECommerceApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApp.Domain.Interfaces.Repositories
{
    public interface IProductImageRepository : IGenericRepository<ProductImage>
    {
        Task<int> CountByProductIdAsync(long productId, CancellationToken cancellationToken = default);
        Task<List<ProductImage>> GetByProductIdAsync(long productId, CancellationToken cancellationToken = default);
        Task<ProductImage?> GetByIdWithProductAsync(long id, CancellationToken cancellationToken = default);

    }
}
