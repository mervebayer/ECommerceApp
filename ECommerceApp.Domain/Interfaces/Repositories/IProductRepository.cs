using ECommerceApp.Domain.Entities;
using ECommerceApp.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ECommerceApp.Domain.Interfaces.Repositories
{
    public interface IProductRepository : IGenericRepository<Product>
    {
        Task<IEnumerable<Product>> GetProductList(int pageSize, int pageNumber, ProductSortType sortType, CancellationToken cancellationToken = default);
        Task<IEnumerable<Product>> GetAllWithCategoriesWithoutImageAsync(int pageSize, int pageNumber, ProductSortType sortType, CancellationToken cancellationToken = default);
        Task<Product> GetProductByIdAsync(long id, CancellationToken cancellationToken = default);
        Task<IEnumerable<Product>> GetProductsByCategoryIdAsync(long categoryId, int pageSize, int pageNumber, ProductSortType sortType = ProductSortType.Newest, CancellationToken cancellationToken = default);        
        Task<Product?> GetByIdWithImagesAsync(long id, CancellationToken cancellationToken = default);
    }
}
