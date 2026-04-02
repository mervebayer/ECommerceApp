using ECommerceApp.Application.DTOs.UserFavorites;
using ECommerceApp.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ECommerceApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class FavoritesController : ControllerBase
    {
        private readonly IUserFavoriteService _userFavoriteService;

        public FavoritesController(IUserFavoriteService userFavoriteService)
        {
            _userFavoriteService = userFavoriteService;
        }

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<FavoriteItemDto>>> GetMyFavorites(CancellationToken cancellationToken)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized();

            var result = await _userFavoriteService.GetMyFavoritesAsync(userId, cancellationToken);
            return Ok(result);
        }

        [HttpGet("{productId:long}/status")]
        public async Task<ActionResult<bool>> IsFavorite(long productId, CancellationToken cancellationToken)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized();

            var result = await _userFavoriteService.IsFavoriteAsync(userId, productId, cancellationToken);
            return Ok(result);
        }

        [HttpPost("{productId:long}")]
        public async Task<IActionResult> AddToFavorites(long productId, CancellationToken cancellationToken)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized();

            await _userFavoriteService.AddToFavoritesAsync(userId, productId, cancellationToken);
            return NoContent();
        }

        [HttpDelete("{productId:long}")]
        public async Task<IActionResult> RemoveFromFavorites(long productId, CancellationToken cancellationToken)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized();

            await _userFavoriteService.RemoveFromFavoritesAsync(userId, productId, cancellationToken);
            return NoContent();
        }
    }
}
