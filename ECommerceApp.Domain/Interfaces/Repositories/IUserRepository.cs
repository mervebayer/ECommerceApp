using ECommerceApp.Domain.Entities;
using ECommerceApp.Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApp.Application.Interfaces
{ 
    public interface IUserRepository
    {
        Task<AppUser?> GetByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
        Task<AppUser?> GetByIdAsync(string userId, CancellationToken cancellationToken);
        Task<AppUser?> GetByEmailOrUserNameAsync(string emailOrUserName, CancellationToken cancellationToken);

        Task<IList<string>> GetRolesAsync(AppUser user, CancellationToken cancellationToken);
        Task<bool> IsInRoleAsync(AppUser user, string roleName, CancellationToken cancellationToken);

        Task AddToRoleAsync(AppUser user, string roleName, CancellationToken cancellationToken);
        Task RemoveFromRoleAsync(AppUser user, string roleName, CancellationToken cancellationToken);

        Task<IReadOnlyList<AppUser>> GetAllAsync(CancellationToken cancellationToken);

        Task<Dictionary<string, IList<string>>> GetRolesByUserIdsAsync(IEnumerable<string> userIds, CancellationToken cancellationToken);
    }
}
