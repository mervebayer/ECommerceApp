using ECommerceApp.Application.Interfaces;
using System.Security.Claims;

namespace ECommerceApp.API.Services
{
    public class BasketIdentityService : IBasketIdentityService
    {
        private const string BasketCookieName = "basketId";

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IBasketService _basketService;

        public BasketIdentityService(IHttpContextAccessor httpContextAccessor, IBasketService basketService)
        {
            _httpContextAccessor = httpContextAccessor;
            _basketService = basketService;
        }

        public async Task<string> GetOrCreateBasketIdAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var httpContext = _httpContextAccessor.HttpContext ?? throw new InvalidOperationException("HttpContext is not available.");

            var userId = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!string.IsNullOrWhiteSpace(userId))
            {
                var userBasketId = $"user:{userId}";
                var anonymousBasketId = GetAnonymousBasketId(httpContext);

                if (!string.IsNullOrWhiteSpace(anonymousBasketId))
                {
                    await _basketService.MergeBasketsAsync(anonymousBasketId, userBasketId);
                    httpContext.Response.Cookies.Delete(BasketCookieName);
                }

                return userBasketId;
            }

            var basketId = GetAnonymousBasketId(httpContext);

            if (string.IsNullOrWhiteSpace(basketId))
            {
                basketId = Guid.NewGuid().ToString();

                httpContext.Response.Cookies.Append(BasketCookieName, basketId, new CookieOptions
                {
                    Expires = DateTimeOffset.UtcNow.AddDays(30),
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Lax,
                    IsEssential = true
                });
            }

            return basketId;
        }

        private static string? GetAnonymousBasketId(HttpContext httpContext)
        {
            var basketId = httpContext.Request.Cookies[BasketCookieName];
            return Guid.TryParse(basketId, out _) ? basketId : null;
        }
    }
}
