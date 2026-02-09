using ECommerceApp.Core.Entities;
using ECommerceApp.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApp.Core.Interfaces.Repositories
{
    public interface IProductRepository : IGenericRepository<Product>
    {
        Task<IEnumerable<Product>> GetAllWithCategoriesAsync(int pageSize, int pageNumber, ProductSortType sortType);
        Task<Product> GetByIdWithCategoryAsync(long id, CancellationToken cancellationToken = default);
        Task<IEnumerable<Product>> GetProductsByCategoryIdAsync(long categoryId, int pageSize, int pageNumber, ProductSortType sortType);
    }
}
