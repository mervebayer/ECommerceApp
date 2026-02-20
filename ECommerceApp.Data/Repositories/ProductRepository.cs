using ECommerceApp.Core.DTOs.Products;
using ECommerceApp.Core.Entities;
using ECommerceApp.Core.Enums;
using ECommerceApp.Core.Extensions;
using ECommerceApp.Data;
using ECommerceApp.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;


namespace ECommerceApp.Core.Interfaces.Repositories
{
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        private readonly AppDbContext _context;

        public ProductRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        // TODO: CQRS
        public async Task<IEnumerable<ProductListDto>> GetProductList(int pageSize, int pageNumber, ProductSortType sortType, CancellationToken cancellationToken = default)
        {
            IQueryable<Product> query = _context.Products.AsNoTracking().Include(x => x.Category);

            query = query.SortBy(sortType).ToPagedList(pageNumber, pageSize);

            return await query.Select(p => new ProductListDto
            {
                Id = p.Id,
                Name = p.Name,
                Stock = p.Stock,
                CategoryId = p.CategoryId,
                CategoryName = p.Category.Name,
                MainImageUrl = p.Images.Where(i => i.IsMain).Select(i => i.Url).FirstOrDefault(),
                CreatedDate = p.CreatedDate,
                UpdatedDate = p.UpdatedDate
            }).ToListAsync(cancellationToken);

        }

        public async Task<Product?> GetProductByIdAsync(long id, CancellationToken cancellationToken = default)
        {
            return await _context.Products.Include(x => x.Category).Include(x => x.Images).SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<IEnumerable<ProductListDto>> GetProductsByCategoryIdAsync(long categoryId, int pageSize, int pageNumber, ProductSortType sortType = ProductSortType.Newest, CancellationToken cancellationToken = default)
        {
            IQueryable<Product> query = _context.Products.AsNoTracking().Include(x => x.Category).Where(x => x.CategoryId == categoryId);

            query = query.SortBy(sortType).ToPagedList(pageNumber, pageSize);

            return await query.Select(p => new ProductListDto
            {
                Id = p.Id,
                Name = p.Name,
                Stock = p.Stock,
                CategoryId = p.CategoryId,
                CategoryName = p.Category.Name,
                MainImageUrl = p.Images.Where(i => i.IsMain).Select(i => i.Url).FirstOrDefault(),
                CreatedDate = p.CreatedDate,
                UpdatedDate = p.UpdatedDate
            }).ToListAsync(cancellationToken);     
        }

        public async Task<IEnumerable<Product>> GetAllWithCategoriesWithoutImageAsync(int pageSize, int pageNumber, ProductSortType sortType, CancellationToken cancellationToken = default)
        {
            IQueryable<Product> query = _context.Products.AsNoTracking().Include(x => x.Category);

            return await query.SortBy(sortType)
                        .ToPagedList(pageNumber, pageSize)
                        .ToListAsync(cancellationToken);
        }

    }
}
