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
            switch (sortType)
            {
                case ProductSortType.Newest:
                    query = query.OrderByDescending(x => x.CreatedDate);
                    break;
                case ProductSortType.PriceDesc:
                    query = query.OrderByDescending(x => x.Price);
                    break;
                case ProductSortType.PriceAsc:
                    query = query.OrderBy(x => x.Price);
                    break;
                case ProductSortType.NameAsc:
                    query = query.OrderBy(x => x.Name);
                    break;
                default:
                    query = query.OrderByDescending(x => x.CreatedDate);
                    break;

            }
            return await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
         
        }

        public async Task<Product> GetByIdWithCategoryAsync(long id, CancellationToken cancellationToken)
        {
            return await _context.Products.Include(x => x.Category).SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<IEnumerable<Product>> GetProductsByCategoryIdAsync(long categoryId, int pageSize, int pageNumber, ProductSortType sortType = ProductSortType.Newest)
        {
            var query = _context.Products.Include(x => x.Category).Where(x => x.CategoryId == categoryId);

            switch (sortType) {
                case ProductSortType.Newest:
                    query = query.OrderByDescending(x => x.CreatedDate);
                    break;
                case ProductSortType.PriceDesc:
                    query = query.OrderByDescending(x => x.Price);
                    break;
                case ProductSortType.PriceAsc:
                    query = query.OrderBy(x => x.Price);
                    break;
                case ProductSortType.NameAsc:
                    query = query.OrderBy(x => x.Name);
                    break;
                default:
                    query = query.OrderByDescending(x => x.CreatedDate);
                    break;

                } 
                           
               return await query.Skip((pageNumber-1) * pageSize).Take(pageSize).ToListAsync();
        }

        public IQueryable<Product> Where(Expression<Func<Product, bool>> expression)
        {
            throw new NotImplementedException();
        }
    }
}
