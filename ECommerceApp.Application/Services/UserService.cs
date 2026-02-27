using ECommerceApp.Application.DTOs.Users;
using ECommerceApp.Application.Interfaces;
using ECommerceApp.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace ECommerceApp.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UserService> _logger;

        public UserService(IUserRepository userRepository, ILogger<UserService> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        // TODO(perf): N+1 issue: roles are loaded per user.
        // add pagination or cache lookup or 2-query approach
        public async Task<IReadOnlyList<UserListItemDto>> GetUsersAsync(CancellationToken cancellationToken)
        {
            var users = await _userRepository.GetAllAsync(cancellationToken);

            var result = new List<UserListItemDto>(users.Count);

            foreach (var u in users)
            {
                var roles = await _userRepository.GetRolesAsync(u, cancellationToken);
                result.Add(new UserListItemDto(
                    u.Id,
                    u.UserName ?? "",
                    u.Email ?? "",
                    u.FirstName ?? "",
                    u.LastName ?? "",
                    roles
                ));
            }

            return result;
        }

        public async Task<UserDetailDto> GetUserByIdAsync(string userId, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(userId, cancellationToken)
                ?? throw new NotFoundException("User not found.");

            var roles = await _userRepository.GetRolesAsync(user, cancellationToken);

            return new UserDetailDto(
                user.Id,
                user.UserName ?? "",
                user.Email ?? "",
                user.FirstName ?? "",
                user.LastName ?? "",
                roles
            );
        }

        public async Task<IList<string>> GetUserRolesAsync(string userId, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(userId, cancellationToken)
                ?? throw new NotFoundException("User not found.");

            return await _userRepository.GetRolesAsync(user, cancellationToken);
        }

        public async Task AssignRoleAsync(string userId, string roleName, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(roleName))
            {
                _logger.LogWarning("AssignRole failed. RoleName is empty. UserId={UserId}", userId);
                throw new BadRequestException("RoleName is required.");
            }

            var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
            if (user is null)
            {
                _logger.LogWarning("AssignRole failed. User not found. UserId={UserId}", userId);

                throw new NotFoundException("User not found.");
            }

            var alreadyInRole = await _userRepository.IsInRoleAsync(user, roleName, cancellationToken);
            if (alreadyInRole)
            {
                _logger.LogWarning( "AssignRole skipped. User already in role. UserId={UserId}, Role={Role}", userId, roleName);
                throw new BadRequestException("User already has this role.");
            }

            await _userRepository.AddToRoleAsync(user, roleName, cancellationToken);
            _logger.LogInformation("Role assigned successfully. UserId={UserId}, Role={Role}", userId, roleName);
        }

        public async Task RemoveRoleAsync(string userId, string roleName, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(roleName))
                throw new BadRequestException("RoleName is required.");

            var user = await _userRepository.GetByIdAsync(userId, cancellationToken)
                ?? throw new NotFoundException("User not found.");

            var inRole = await _userRepository.IsInRoleAsync(user, roleName, cancellationToken);
            if (!inRole)
                throw new BadRequestException("User does not have this role.");

            await _userRepository.RemoveFromRoleAsync(user, roleName, cancellationToken);
            _logger.LogInformation("Role removed successfully. UserId={UserId}, Role={Role}", userId, roleName);
        }
    }
}