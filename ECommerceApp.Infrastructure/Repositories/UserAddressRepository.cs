using ECommerceApp.Application.Interfaces;
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
    public class UserAddressRepository : GenericRepository<UserAddress>, IUserAddressRepository
    {
        private readonly AppDbContext _context;

        public UserAddressRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<UserAddress?> GetByIdAndUserIdAsync(long addressId, string userId, CancellationToken cancellationToken = default)
        {
            return await _context.UserAddresses.SingleOrDefaultAsync(x => x.Id == addressId && x.UserId == userId, cancellationToken);
        }

        public async Task<List<UserAddress>> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default)
        {
            return await _context.UserAddresses.Where(x => x.UserId == userId).OrderByDescending(x => x.IsDefault)
                .ThenByDescending(x => x.CreatedDate).ToListAsync(cancellationToken);
        }

        public async Task<UserAddress?> GetDefaultByUserIdAsync(string userId, CancellationToken cancellationToken = default)
        {
            return await _context.UserAddresses.SingleOrDefaultAsync(x => x.UserId == userId && x.IsDefault, cancellationToken);
        }
    }
}
