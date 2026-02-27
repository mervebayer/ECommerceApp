using ECommerceApp.Application.DTOs.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApp.Application.Interfaces
{
    public interface IUserService
    {
        Task<IReadOnlyList<UserListItemDto>> GetUsersAsync(CancellationToken cancellationToken);
        Task<UserDetailDto> GetUserByIdAsync(string userId, CancellationToken cancellationToken);

        Task AssignRoleAsync(string userId, string roleName, CancellationToken cancellationToken);
        Task RemoveRoleAsync(string userId, string roleName, CancellationToken cancellationToken);

        Task<IList<string>> GetUserRolesAsync(string userId, CancellationToken cancellationToken);
    }
}
