using ECommerceApp.Application.DTOs.Products;
using ECommerceApp.Application.DTOs.QueryParams;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApp.Application.Interfaces
{
    public interface IProductReadRepository
    {
        Task<PagedResult<ProductListItemDto>> GetProductsAsync(ProductQueryParams p, CancellationToken cancellationToken);
    }
}
