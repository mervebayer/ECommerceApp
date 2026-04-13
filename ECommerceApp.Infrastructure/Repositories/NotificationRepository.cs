using ECommerceApp.Domain.Entities;
using ECommerceApp.Domain.Enums;
using ECommerceApp.Domain.Interfaces.Repositories;
using ECommerceApp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApp.Infrastructure.Repositories
{
    public class NotificationRepository : GenericRepository<Notification>, INotificationRepository
    {
        private readonly AppDbContext _context;

        public NotificationRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<Notification>> GetUserNotificationsAsync(string userId, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            return await _context.Notifications.AsNoTracking().Where(x => x.ReceiverUserId == userId && x.Audience == NotificationAudience.Customer)
                .OrderByDescending(x => x.CreatedDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<Notification>> GetStaffNotificationsAsync(string userId, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            return await _context.Notifications.AsNoTracking().Where(x => x.ReceiverUserId == userId && x.Audience == NotificationAudience.Backoffice)
                .OrderByDescending(x => x.CreatedDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);
        }

        public async Task<int> GetUnreadCountForUserAsync(string userId, CancellationToken cancellationToken = default)
        {
            return await _context.Notifications.AsNoTracking().CountAsync(x => x.ReceiverUserId == userId && x.Audience == NotificationAudience.Customer && !x.IsRead, cancellationToken);
        }

        public async Task<int> GetUnreadCountForStaffAsync(string userId, CancellationToken cancellationToken = default)
        {
            return await _context.Notifications.AsNoTracking().CountAsync(x => x.ReceiverUserId == userId && x.Audience == NotificationAudience.Backoffice && !x.IsRead, cancellationToken);
        }

        public async Task<Notification?> GetByIdForUserAsync(long notificationId, string userId, CancellationToken cancellationToken = default)
        {
            return await _context.Notifications.SingleOrDefaultAsync(x => x.Id == notificationId && x.ReceiverUserId == userId && x.Audience == NotificationAudience.Customer, cancellationToken);
        }

        public async Task<Notification?> GetByIdForStaffAsync(long notificationId, string userId, CancellationToken cancellationToken = default)
        {
            return await _context.Notifications.SingleOrDefaultAsync(x => x.Id == notificationId && x.ReceiverUserId == userId && x.Audience == NotificationAudience.Backoffice, cancellationToken);
        }

        public async Task<int> CountForUserAsync(string userId, CancellationToken cancellationToken = default) 
        { 
            return await _context.Notifications.AsNoTracking().CountAsync(x => x.ReceiverUserId == userId && x.Audience == NotificationAudience.Customer, cancellationToken); 
        }
        public async Task<int> CountForStaffAsync(string userId, CancellationToken cancellationToken = default)
        {
            return await _context.Notifications.AsNoTracking().CountAsync(x => x.ReceiverUserId == userId && x.Audience == NotificationAudience.Backoffice, cancellationToken);
        }
    }
}
