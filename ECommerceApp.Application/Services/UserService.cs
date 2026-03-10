using ECommerceApp.Application.DTOs.Users;
using ECommerceApp.Application.DTOs.QueryParams;
using ECommerceApp.Application.Interfaces;
using ECommerceApp.Domain.Exceptions;
using FluentValidation;
using Microsoft.Extensions.Logging;
using ECommerceApp.Application.Extensions;

namespace ECommerceApp.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UserService> _logger;
        private readonly IValidator<UserQueryParams> _queryParamsValidator;

        public UserService(IUserRepository userRepository, ILogger<UserService> logger, IValidator<UserQueryParams> queryParamsValidator)
        {
            _userRepository = userRepository;
            _logger = logger;
            _queryParamsValidator = queryParamsValidator;
        }

        // Roles are loaded in batch to avoid N+1 queries when listing paginated users.
        public async Task<PagedResult<UserListItemDto>> GetUsersAsync(UserQueryParams queryParams, CancellationToken cancellationToken)
        {
            var validationResult = await _queryParamsValidator.ValidateAsync(queryParams, cancellationToken);
            validationResult.ThrowIfInvalid();

            var totalCount = await _userRepository.CountAsync(cancellationToken);

            if (totalCount == 0)
            {
                return new PagedResult<UserListItemDto>
                {
                    Items = Array.Empty<UserListItemDto>(),
                    TotalCount = 0
                };
            }

            var users = await _userRepository.GetPagedListAsync(queryParams.PageSize, queryParams.PageNumber, cancellationToken);

            var userIds = users.Select(x => x.Id).ToList();
            var rolesByUserId = await _userRepository.GetRolesByUserIdsAsync(userIds, cancellationToken);

            var items = users.Select(u =>
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

            return new PagedResult<UserListItemDto>
            {
                Items = items,
                TotalCount = totalCount
            };
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