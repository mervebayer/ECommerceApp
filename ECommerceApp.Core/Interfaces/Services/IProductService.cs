using ECommerceApp.Core.DTOs.Products;
using ECommerceApp.Core.Entities;
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
        Task<IEnumerable<ProductDto>> GetAllAsync(int pageSize, int pageNumber, ProductSortType sortType);
        Task<ProductDto> GetByIdAsync(long id);
        Task<ProductDto> GetByIdWithoutCategoryAsync(long id);
        Task<ProductDto> AddAsync(ProductCreateDto entity);
        Task<ProductDto> Update(ProductUpdateDto entity);
        Task Delete(long id);
        Task<IEnumerable<ProductDto>> GetProductsByCategoryIdAsync(long categoryId, int pageNumber, int pageSize, ProductSortType sortType);  
          
    }
}
