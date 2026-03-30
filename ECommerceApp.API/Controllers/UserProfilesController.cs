using ECommerceApp.Application.DTOs.UserProfiles;
using ECommerceApp.Application.Interfaces;
using ECommerceApp.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ECommerceApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserProfilesController : ControllerBase
    {
        private readonly IUserProfileService _userProfileService;

        public UserProfilesController(IUserProfileService userProfileService)
        {
            _userProfileService = userProfileService;
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<ActionResult<UserProfileDto>> GetMyProfile(CancellationToken cancellationToken)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized();

            var result = await _userProfileService.GetMyProfileAsync(userId, cancellationToken);
            return Ok(result);
        }

        [Authorize]
        [HttpPut("me")]
        public async Task<ActionResult<UserProfileDto>> UpdateMyProfile([FromBody] UpdateUserProfileDto request, CancellationToken cancellationToken)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized();

            var result = await _userProfileService.UpdateMyProfileAsync(userId, request, cancellationToken);
            return Ok(result);
        }

        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto request, CancellationToken cancellationToken)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized();

            await _userProfileService.ChangePasswordAsync(userId, request, cancellationToken);
            return NoContent();
        }
    }
}
