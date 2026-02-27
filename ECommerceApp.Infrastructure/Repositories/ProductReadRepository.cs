using ECommerceApp.Application.DTOs.Products;
using ECommerceApp.Application.DTOs.QueryParams;
using ECommerceApp.Application.Interfaces;
using ECommerceApp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using ECommerceApp.Application.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApp.Infrastructure.Repositories
{
    public class ProductReadRepository : IProductReadRepository
    {
        private readonly AppDbContext _context;

        public ProductReadRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<ProductListItemDto>> GetProductsAsync(ProductQueryParams p, CancellationToken cancellationToken)
        {
            var query = _context.Products
                .AsNoTracking()
                .AsQueryable();

            if (p.CategoryId is not null)
                query = query.Where(x => x.CategoryId == p.CategoryId);

            if (!string.IsNullOrWhiteSpace(p.Search))
            {
                var s = p.Search.Trim().ToLower();

              //  query = query.Where(x => x.Name.ToLower().Contains(s));

                query = query.Where(x => EF.Functions.Like(x.Name, $"%{p.Search.Trim()}%"));
            }
            if (p.MinPrice > 0)
                query = query.Where(x => x.Price >= p.MinPrice);

            if (p.MaxPrice > 0)
                query = query.Where(x => x.Price <= p.MaxPrice);

            var totalCount = await query.CountAsync(cancellationToken);

            query = query.SortBy(p.SortType).ToPagedList(p.PageNumber, p.PageSize);

            var items = await query
                .Select(x => new ProductListItemDto(
                    x.Id,
                    x.Name,
                    x.Category.Name,
                    x.Images
                        .OrderByDescending(i => i.IsMain)
                        .Select(i => i.Url)
                        .FirstOrDefault()
                        ?? "https://upload.wikimedia.org/wikipedia/commons/1/14/No_Image_Available.jpg",
                    x.Price
                ))
                .ToListAsync(cancellationToken);

            return new PagedResult<ProductListItemDto>
            {
                Items = items,
                TotalCount = totalCount
            };
        }
    }
}
