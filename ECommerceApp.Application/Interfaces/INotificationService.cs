using ECommerceApp.Application.DTOs.Notifications;
using ECommerceApp.Application.DTOs.QueryParams;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApp.Application.Interfaces
{
    public interface INotificationService
    {
        Task<PagedResult<NotificationDto>> GetMyNotificationsAsync(string userId, NotificationQueryParams queryParams, CancellationToken cancellationToken); 
        Task<PagedResult<NotificationDto>> GetRoleNotificationsAsync(string role, NotificationQueryParams queryParams, CancellationToken cancellationToken);
        Task<int> GetUnreadCountForUserAsync(string userId, CancellationToken cancellationToken); 
        Task<int> GetUnreadCountForRoleAsync(string role, CancellationToken cancellationToken); 
        Task MarkAsReadForUserAsync(long notificationId, string userId, CancellationToken cancellationToken); 
        Task MarkAsReadForRoleAsync(long notificationId, string role, CancellationToken cancellationToken);
    }
}
