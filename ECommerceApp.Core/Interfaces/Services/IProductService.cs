using ECommerceApp.Core.DTOs.Products;
using ECommerceApp.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApp.Core.Interfaces.Services
{
    public interface IProductService
    {
        Task<IEnumerable<ProductListDto>> GetAllAsync(int pageSize, int pageNumber, ProductSortType sortType, CancellationToken cancellationToken);
        Task<ProductDto> GetByIdAsync(long id, CancellationToken cancellationToken);
        Task<ProductDto> GetByIdWithoutCategoryAsync(long id, CancellationToken cancellationToken);
        Task<ProductDto> AddAsync(ProductCreateDto entity, CancellationToken cancellationToken);
        //Task<ProductDto> Update(long id, ProductUpdateDto entity);
        Task UpdateAsync(long id, ProductUpdateDto entity, CancellationToken cancellationToken);
        Task DeleteAsync(long id, CancellationToken cancellationToken);
        Task<IEnumerable<ProductListDto>> GetProductsByCategoryIdAsync(long categoryId, int pageNumber, int pageSize, ProductSortType sortType, CancellationToken cancellationToken);  
          
    }
}
