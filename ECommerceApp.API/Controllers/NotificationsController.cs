using ECommerceApp.Application.DTOs.QueryParams;
using ECommerceApp.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ECommerceApp.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class NotificationsController : ControllerBase
{
    private readonly INotificationService _notificationService;

    public NotificationsController(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    [HttpGet("my")]
    public async Task<IActionResult> GetMyNotifications([FromQuery] NotificationQueryParams queryParams, CancellationToken cancellationToken = default)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrWhiteSpace(userId))
            return Unauthorized();

        var result = await _notificationService.GetMyNotificationsAsync(userId, queryParams, cancellationToken);

        return Ok(result);
    }

    [HttpGet("my/unread-count")]
    public async Task<IActionResult> GetMyUnreadCount(CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrWhiteSpace(userId))
            return Unauthorized();

        var count = await _notificationService.GetUnreadCountForUserAsync(userId, cancellationToken);

        return Ok(new { count });
    }

    [HttpPatch("my/{id:long}/read")]
    public async Task<IActionResult> MarkMyNotificationAsRead(long id, CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrWhiteSpace(userId))
            return Unauthorized();

        await _notificationService.MarkAsReadForUserAsync(id, userId, cancellationToken);

        return NoContent();
    }

    [HttpGet("admin")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAdminNotifications([FromQuery] NotificationQueryParams queryParams, CancellationToken cancellationToken = default)
    {
        var result = await _notificationService.GetRoleNotificationsAsync("Admin", queryParams, cancellationToken);

        return Ok(result);
    }

    [HttpGet("admin/unread-count")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAdminUnreadCount(CancellationToken cancellationToken)
    {
        var count = await _notificationService.GetUnreadCountForRoleAsync("Admin", cancellationToken);

        return Ok(new { count });
    }

    [HttpPatch("admin/{id:long}/read")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> MarkAdminNotificationAsRead(long id, CancellationToken cancellationToken)
    {
        await _notificationService.MarkAsReadForRoleAsync(id, "Admin", cancellationToken);

        return NoContent();
    }
}
