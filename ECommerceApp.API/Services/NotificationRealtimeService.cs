using ECommerceApp.API.Hubs;
using ECommerceApp.Application.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace ECommerceApp.API.Services
{
    public class NotificationRealtimeService : INotificationRealtimeService
    {
        private readonly IHubContext<NotificationHub> _hubContext;

        public NotificationRealtimeService(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public Task NotifyBackofficeAsync(string title, string message, long orderId, CancellationToken cancellationToken)
        {
            return _hubContext.Clients.Group("backoffice").SendAsync(
                "NotificationReceived",
                new
                {
                    title,
                    message,
                    orderId,
                    refreshOrders = true
                },
                cancellationToken);
        }

        public Task NotifyUserAsync(string userId, string title, string message, long orderId, CancellationToken cancellationToken)
        {
            return _hubContext.Clients.Group($"user:{userId}").SendAsync(
                "NotificationReceived",
                new
                {
                    title,
                    message,
                    orderId,
                    refreshOrders = false
                },
                cancellationToken);
        }

        public Task NotifyOrderUpdatedAsync(string backofficeTitle, string backofficeMessage, string userId, string userTitle, string userMessage, long orderId, CancellationToken cancellationToken)
        {
            return Task.WhenAll(
                _hubContext.Clients.Group("backoffice").SendAsync(
                    "OrderUpdated",
                    new
                    {
                        title = backofficeTitle,
                        message = backofficeMessage,
                        orderId,
                        refreshOrders = true
                    },
                    cancellationToken),
                _hubContext.Clients.Group($"user:{userId}").SendAsync(
                    "OrderUpdated",
                    new
                    {
                        title = userTitle,
                        message = userMessage,
                        orderId,
                        refreshOrders = false
                    },
                    cancellationToken));
        }
    }
}
