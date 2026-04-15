using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace ECommerceApp.API.Hubs
{
    [Authorize]
    public class NotificationHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            var userId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!string.IsNullOrWhiteSpace(userId))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, $"user:{userId}");
            }

            if (Context.User?.IsInRole("Admin") == true || Context.User?.IsInRole("StoreManager") == true)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, "backoffice");
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!string.IsNullOrWhiteSpace(userId))
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"user:{userId}");
            }

            if (Context.User?.IsInRole("Admin") == true || Context.User?.IsInRole("StoreManager") == true)
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, "backoffice");
            }

            await base.OnDisconnectedAsync(exception);
        }
    }
}
