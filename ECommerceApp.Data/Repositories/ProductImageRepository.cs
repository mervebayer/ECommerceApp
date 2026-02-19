using ECommerceApp.Core.Entities;
using ECommerceApp.Core.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApp.Data.Repositories
{
    public class ProductImageRepository : GenericRepository<ProductImage>, IProductImageRepository
    {
        private readonly AppDbContext _context;

        public ProductImageRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<int> CountByProductIdAsync(long productId) {
            return await _context.ProductImages.CountAsync(x => x.ProductId == productId);
        }

        public async Task<List<ProductImage>> GetByProductIdAsync(long productId)
        {
            return await _context.ProductImages
                .Where(x => x.ProductId == productId) 
                .OrderByDescending(x => x.IsMain)
                .ThenByDescending(x => x.Id)
                .ToListAsync(); 
        }

        public async Task<ProductImage?> GetByIdWithProductAsync(long id)
        { 
            return await _context.ProductImages.Include(x => x.Product)
                        .FirstOrDefaultAsync(x => x.Id == id);
        }
    }

}
