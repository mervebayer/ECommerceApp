using ECommerceApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApp.Domain.Interfaces.Repositories
{
    public interface INotificationRepository : IGenericRepository<Notification>
    {
        Task<IReadOnlyList<Notification>> GetUserNotificationsAsync(string userId, int pageNumber, int pageSize, CancellationToken cancellationToken = default);

        Task<IReadOnlyList<Notification>> GetRoleNotificationsAsync(string role, int pageNumber, int pageSize, CancellationToken cancellationToken = default);

        Task<int> GetUnreadCountForUserAsync(string userId, CancellationToken cancellationToken = default);

        Task<int> GetUnreadCountForRoleAsync(string role, CancellationToken cancellationToken = default);

        Task<Notification?> GetByIdForUserAsync(long notificationId, string userId, CancellationToken cancellationToken = default);

        Task<Notification?> GetByIdForRoleAsync(long notificationId, string role, CancellationToken cancellationToken = default);

        Task<int> CountForUserAsync(string userId, CancellationToken cancellationToken = default);

        Task<int> CountForRoleAsync(string role, CancellationToken cancellationToken = default);

    }
}
