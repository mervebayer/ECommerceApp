using ECommerceApp.Application.DTOs.Products;
using ECommerceApp.Application.DTOs.QueryParams;
using ECommerceApp.Domain.Entities;
using ECommerceApp.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApp.Application.Interfaces
{
    public interface IProductService
    {
        Task<PagedResult<ProductListItemDto>> GetAllAsync(ProductQueryParams p, CancellationToken cancellationToken);
        Task<ProductDto> GetByIdAsync(long id, CancellationToken cancellationToken);
        Task<ProductDto> GetByIdWithoutCategoryAsync(long id, CancellationToken cancellationToken);
        Task<ProductDto> AddAsync(ProductCreateDto entity, CancellationToken cancellationToken);
        //Task<ProductDto> Update(long id, ProductUpdateDto entity);
        Task UpdateAsync(long id, ProductUpdateDto entity, CancellationToken cancellationToken);
        Task DeleteAsync(long id, CancellationToken cancellationToken);
        Task<IEnumerable<ProductListDto>> GetProductsByCategoryIdAsync(long categoryId, int pageNumber, int pageSize, ProductSortType sortType, CancellationToken cancellationToken);
        Task<GenerateProductDescriptionResponseDto> GenerateDescriptionAsync(long id, CancellationToken cancellationToken);
        Task<ProductDto> ApplyGeneratedDescriptionAsync(long id, ApplyGeneratedDescriptionRequestDto request, CancellationToken cancellationToken);

    }
}
