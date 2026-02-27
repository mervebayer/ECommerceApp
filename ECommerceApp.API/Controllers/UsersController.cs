using ECommerceApp.Application.DTOs.Users;
using ECommerceApp.Application.Interfaces;
using ECommerceApp.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Roles = "Admin")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }


        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<UserListItemDto>>> GetUsers(CancellationToken cancellationToken)
        {
            var x = _userService;
            var users = await _userService.GetUsersAsync(cancellationToken);
            return Ok(users);
        }

        [HttpGet("{userId}")]
        public async Task<ActionResult<UserDetailDto>> GetUserById(string userId, CancellationToken cancellationToken)
        {
            var user = await _userService.GetUserByIdAsync(userId, cancellationToken);
            return Ok(user);
        }

        [HttpGet("{userId}/roles")]
        public async Task<ActionResult<IList<string>>> GetUserRoles(string userId, CancellationToken cancellationToken)
        {
            var roles = await _userService.GetUserRolesAsync(userId, cancellationToken);
            return Ok(roles);
        }

        [HttpPost("{userId}/roles")]
        public async Task<IActionResult> AssignRole(string userId, [FromBody] AssignRoleDto dto, CancellationToken cancellationToken)
        {
            await _userService.AssignRoleAsync(userId, dto.RoleName, cancellationToken);
            return NoContent();
        }

        [HttpDelete("{userId}/roles/{roleName}")]
        public async Task<IActionResult> RemoveRole(string userId, string roleName, CancellationToken cancellationToken)
        {
            await _userService.RemoveRoleAsync(userId, roleName, cancellationToken);
            return NoContent();
        }
    }
}
