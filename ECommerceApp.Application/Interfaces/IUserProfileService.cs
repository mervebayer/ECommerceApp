using ECommerceApp.Application.DTOs.UserProfiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApp.Application.Interfaces
{
    public interface IUserProfileService
    {
        Task<UserProfileDto> GetMyProfileAsync(string userId, CancellationToken cancellationToken = default);
        Task<UserProfileDto> UpdateMyProfileAsync(string userId, UpdateUserProfileDto request, CancellationToken cancellationToken = default);
        Task ChangePasswordAsync(string userId, ChangePasswordDto request, CancellationToken cancellationToken = default);
    }
}
