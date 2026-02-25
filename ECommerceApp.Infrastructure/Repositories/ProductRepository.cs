using ECommerceApp.Domain.Entities;
using ECommerceApp.Domain.Enums;
using ECommerceApp.Domain.Interfaces.Repositories;
using ECommerceApp.Infrastructure.Extensions;
using ECommerceApp.Infrastructure.Persistence;
using ECommerceApp.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;


namespace ECommerceApp.Infrastructure.Interfaces.Repositories
{
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        private readonly AppDbContext _context;

        public ProductRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        // TODO: CQRS
        public async Task<IEnumerable<Product>> GetProductList( int pageSize, int pageNumber, ProductSortType sortType, CancellationToken cancellationToken = default)
        {
            IQueryable<Product> query = _context.Products
                .AsNoTracking()
                .Include(x => x.Category)
                .Include(x => x.Images); 

            query = query.SortBy(sortType).ToPagedList(pageNumber, pageSize);

            return await query.ToListAsync(cancellationToken);
        }

        public async Task<Product?> GetProductByIdAsync(long id, CancellationToken cancellationToken = default)
        {
            return await _context.Products.Include(x => x.Category).Include(x => x.Images).SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<Product?> GetByIdWithImagesAsync(long id, CancellationToken cancellationToken = default)
        {
            return await _context.Products
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        }


        public async Task<IEnumerable<Product>> GetProductsByCategoryIdAsync(long categoryId, int pageSize, int pageNumber, ProductSortType sortType = ProductSortType.Newest, CancellationToken cancellationToken = default)
        {
            IQueryable<Product> query = _context.Products
                .AsNoTracking()
                .Where(x => x.CategoryId == categoryId)
                .Include(x => x.Category)
                .Include(x => x.Images); 

            query = query.SortBy(sortType).ToPagedList(pageNumber, pageSize);

            return await query.ToListAsync(cancellationToken);
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
