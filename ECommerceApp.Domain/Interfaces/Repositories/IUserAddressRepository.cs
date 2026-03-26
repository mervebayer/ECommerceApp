using ECommerceApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApp.Domain.Interfaces.Repositories
{
    public interface IUserAddressRepository : IGenericRepository<UserAddress>
    {
        Task<List<UserAddress>> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default);
        Task<UserAddress?> GetByIdAndUserIdAsync(long addressId, string userId, CancellationToken cancellationToken = default);
        Task<UserAddress?> GetDefaultByUserIdAsync(string userId, CancellationToken cancellationToken = default);

    }
}
