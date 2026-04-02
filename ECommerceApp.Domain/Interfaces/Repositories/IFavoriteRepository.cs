using ECommerceApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApp.Domain.Interfaces.Repositories
{
    public interface IFavoriteRepository : IGenericRepository<UserFavorite>
    {
        Task<List<UserFavorite>> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default);
        Task<UserFavorite?> GetByUserIdAndProductIdAsync(string userId, long productId, CancellationToken cancellationToken = default);
        Task<bool> ExistsAsync(string userId, long productId, CancellationToken cancellationToken = default);
        void Remove(UserFavorite favorite);

    }
}
