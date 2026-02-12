using ECommerceApp.Core.Entities;
using ECommerceApp.Core.Enums;
using ECommerceApp.Data;
using ECommerceApp.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using ECommerceApp.Core.Extensions;


namespace ECommerceApp.Core.Interfaces.Repositories
{
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        private readonly AppDbContext _context;

        public ProductRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        
        public async Task<IEnumerable<Product>> GetAllWithCategoriesAsync(int pageSize, int pageNumber, ProductSortType sortType)
        {
            IQueryable<Product> query = _context.Products.Include(x => x.Category);

            return await query.SortBy(sortType)
                        .ToPagedList(pageNumber, pageSize)
                        .ToListAsync();
        }

        public async Task<Product> GetByIdWithCategoryAsync(long id, CancellationToken cancellationToken)
        {
            return await _context.Products.Include(x => x.Category).SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<IEnumerable<Product>> GetProductsByCategoryIdAsync(long categoryId, int pageSize, int pageNumber, ProductSortType sortType = ProductSortType.Newest)
        {
            var query = _context.Products.Include(x => x.Category).Where(x => x.CategoryId == categoryId);

            return await query.SortBy(sortType)
                              .ToPagedList(pageNumber, pageSize)
                              .ToListAsync();         
        }

    }
}
