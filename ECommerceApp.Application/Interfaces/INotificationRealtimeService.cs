using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApp.Application.Interfaces
{
    public interface INotificationRealtimeService
    {
        Task NotifyBackofficeAsync(string title, string message, long orderId, CancellationToken cancellationToken);
        Task NotifyUserAsync(string userId, string title, string message, long orderId, CancellationToken cancellationToken);
        Task NotifyOrderUpdatedAsync(string backofficeTitle, string backofficeMessage, string userId, string userTitle, string userMessage, long orderId, CancellationToken cancellationToken);
    }
}
