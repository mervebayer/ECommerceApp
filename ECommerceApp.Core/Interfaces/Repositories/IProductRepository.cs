using ECommerceApp.Core.DTOs.Products;
using ECommerceApp.Core.Entities;
using ECommerceApp.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ECommerceApp.Core.Interfaces.Repositories
{
    public interface IProductRepository : IGenericRepository<Product>
    {
        Task<IEnumerable<ProductListDto>> GetProductList(int pageSize, int pageNumber, ProductSortType sortType, CancellationToken cancellationToken = default);
        Task<IEnumerable<Product>> GetAllWithCategoriesWithoutImageAsync(int pageSize, int pageNumber, ProductSortType sortType, CancellationToken cancellationToken = default);
        Task<Product> GetProductByIdAsync(long id, CancellationToken cancellationToken = default);
        Task<IEnumerable<ProductListDto>> GetProductsByCategoryIdAsync(long categoryId, int pageSize, int pageNumber, ProductSortType sortType, CancellationToken cancellationToken = default);
        Task<Product?> GetByIdWithImagesAsync(long id, CancellationToken cancellationToken = default);
    }
}
