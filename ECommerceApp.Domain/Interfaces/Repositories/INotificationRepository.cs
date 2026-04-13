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
        Task<IReadOnlyList<Notification>> GetStaffNotificationsAsync(string userId, int pageNumber, int pageSize, CancellationToken cancellationToken = default);

        Task<int> GetUnreadCountForUserAsync(string userId, CancellationToken cancellationToken = default);
        Task<int> GetUnreadCountForStaffAsync(string userId, CancellationToken cancellationToken = default);

        Task<Notification?> GetByIdForUserAsync(long notificationId, string userId, CancellationToken cancellationToken = default);
        Task<Notification?> GetByIdForStaffAsync(long notificationId, string userId, CancellationToken cancellationToken = default);

        Task<int> CountForUserAsync(string userId, CancellationToken cancellationToken = default);
        Task<int> CountForStaffAsync(string userId, CancellationToken cancellationToken = default);
    }
}
