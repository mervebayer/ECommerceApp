using ECommerceApp.Application.DTOs.UserFavorites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApp.Application.Interfaces
{
    public interface IUserFavoriteService
    {
        Task<IReadOnlyList<FavoriteItemDto>> GetMyFavoritesAsync(string userId, CancellationToken cancellationToken = default);
        Task AddToFavoritesAsync(string userId, long productId, CancellationToken cancellationToken = default);
        Task RemoveFromFavoritesAsync(string userId, long productId, CancellationToken cancellationToken = default);
        Task<bool> IsFavoriteAsync(string userId, long productId, CancellationToken cancellationToken = default);
    }
    
}
