using ECommerceApp.Application.Extensions;
using ECommerceApp.Application.Interfaces;
using ECommerceApp.Domain.Entities;
using ECommerceApp.Domain.Exceptions;
using ECommerceApp.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApp.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly AppDbContext _context;

        public UserRepository(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, AppDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }


        public async Task<AppUser?> GetByIdAsync(string userId, CancellationToken cancellationToken)
        {
            return await _userManager.FindByIdAsync(userId);
        }

        public async Task<AppUser?> GetByEmailOrUserNameAsync(string emailOrUserName, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(emailOrUserName);
            if (user != null) return user;

            return await _userManager.FindByNameAsync(emailOrUserName);
        }

        public async Task<IReadOnlyList<AppUser>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await _userManager.Users
                .AsNoTracking()
                .OrderBy(u => u.UserName)
                .ToListAsync(cancellationToken);
        }

        public async Task<IList<string>> GetRolesAsync(AppUser user, CancellationToken cancellationToken)
        {
            return await _userManager.GetRolesAsync(user);
        }

        public async Task<bool> IsInRoleAsync(AppUser user, string roleName, CancellationToken cancellationToken)
        {
            return await _userManager.IsInRoleAsync(user, roleName);
        }

        public async Task AddToRoleAsync(AppUser user, string roleName, CancellationToken cancellationToken)
        {
            if (!await _roleManager.RoleExistsAsync(roleName))
                throw new NotFoundException("Role not found.");

            var result = await _userManager.AddToRoleAsync(user, roleName);
            if (!result.Succeeded)
                throw new BadRequestException(string.Join(" | ", result.Errors.Select(e => e.Description)));
        }

        public async Task RemoveFromRoleAsync(AppUser user, string roleName, CancellationToken cancellationToken)
        {
            if (!await _roleManager.RoleExistsAsync(roleName))
                throw new NotFoundException("Role not found.");

            var result = await _userManager.RemoveFromRoleAsync(user, roleName);
            if (!result.Succeeded)
                throw new BadRequestException(string.Join(" | ", result.Errors.Select(e => e.Description)));
        }

        public Task<AppUser?> GetByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
        {
            return _context.Users.SingleOrDefaultAsync(u => u.RefreshToken == refreshToken, cancellationToken);
        }
        public async Task<Dictionary<string, IList<string>>> GetRolesByUserIdsAsync(IEnumerable<string> userIds, CancellationToken cancellationToken)
        {
            var ids = userIds
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct()
                .ToList();

            if (ids.Count == 0)
                return new Dictionary<string, IList<string>>();
        
            var roleMappings = await (
                from userRole in _context.UserRoles
                join role in _context.Roles on userRole.RoleId equals role.Id
                where ids.Contains(userRole.UserId)
                select new
                {
                    userRole.UserId,
                    RoleName = role.Name
                })
                .ToListAsync(cancellationToken);

            return roleMappings
                .GroupBy(x => x.UserId)
                .ToDictionary(
                    g => g.Key,
                    g => (IList<string>)g
                        .Select(x => x.RoleName)
                        .Where(x => !string.IsNullOrWhiteSpace(x))
                        .ToList()
                );
        }

        public async Task<IReadOnlyList<AppUser>> GetPagedListAsync(int pageSize, int pageNumber, CancellationToken cancellationToken)
        {
            IQueryable<AppUser> query = _context.Users
                .AsNoTracking()
                .OrderBy(x => x.UserName);

            query = query.ToPagedList(pageNumber, pageSize);

            return await query.ToListAsync(cancellationToken);
        }

        public async Task<int> CountAsync(CancellationToken cancellationToken)
        {
            return await _context.Users.CountAsync(cancellationToken);
        }
    }
}