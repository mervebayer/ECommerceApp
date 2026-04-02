using ECommerceApp.Domain.Entities;
using ECommerceApp.Domain.Interfaces.Repositories;
using ECommerceApp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApp.Infrastructure.Repositories
{
    public class FavoriteRepository : GenericRepository<UserFavorite>, IFavoriteRepository
    {
        private readonly AppDbContext _context;

        public FavoriteRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<bool> ExistsAsync(string userId, long productId, CancellationToken cancellationToken = default)
        {
            return await _context.UserFavorites.AnyAsync(x => x.UserId == userId && x.ProductId == productId, cancellationToken);
        }

        public async Task<UserFavorite?> GetByUserIdAndProductIdAsync(string userId, long productId, CancellationToken cancellationToken = default)
        {
            return await _context.UserFavorites.SingleOrDefaultAsync(x => x.UserId == userId && x.ProductId == productId, cancellationToken);
        }

        public async Task<List<UserFavorite>> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default)
        {
            return await _context.UserFavorites.Where(x => x.UserId == userId).Include(x => x.Product).ThenInclude(x => x.Images).OrderByDescending(x => x.CreatedDate).ToListAsync(cancellationToken);
        }

        public void Remove(UserFavorite favorite)
        {
            _context.UserFavorites.Remove(favorite);
        }
    }
}
