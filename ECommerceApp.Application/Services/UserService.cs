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

        // Note: Roles are loaded in batch to avoid N+1 queries.
        //  TODO(perf): Pagination can be added later for large user lists.
        public async Task<IReadOnlyList<UserListItemDto>> GetUsersAsync(CancellationToken cancellationToken)
        {
            var users = await _userRepository.GetAllAsync(cancellationToken);

            if (users.Count == 0)
                return Array.Empty<UserListItemDto>();

            var userIds = users.Select(x => x.Id).ToList();
            var rolesByUserId = await _userRepository.GetRolesByUserIdsAsync(userIds, cancellationToken);

            var result = users.Select(u =>
            {
                rolesByUserId.TryGetValue(u.Id, out var roles);
                roles ??= new List<string>();

                return new UserListItemDto(
                    u.Id,
                    u.UserName ?? string.Empty,
                    u.Email ?? string.Empty,
                    u.FirstName ?? string.Empty,
                    u.LastName ?? string.Empty,
                    roles
                );
            }).ToList();

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