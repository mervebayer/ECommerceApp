using ECommerceApp.Application.DTOs.Notifications;
using ECommerceApp.Application.DTOs.QueryParams;
using ECommerceApp.Application.Interfaces;
using ECommerceApp.Domain.Exceptions;
using ECommerceApp.Domain.Interfaces;
using ECommerceApp.Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApp.Application.Services
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly IUnitOfWork _unitOfWork;

        public NotificationService(INotificationRepository notificationRepository, IUnitOfWork unitOfWork)
        {
            _notificationRepository = notificationRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<PagedResult<NotificationDto>> GetMyNotificationsAsync(string userId, NotificationQueryParams queryParams, CancellationToken cancellationToken)
        {
            var totalCount = await _notificationRepository.CountForUserAsync(userId, cancellationToken);

            if (totalCount == 0)
            {
                return new PagedResult<NotificationDto>
                {
                    Items = Array.Empty<NotificationDto>(),
                    TotalCount = 0
                };
            }

            var notifications = await _notificationRepository.GetUserNotificationsAsync(userId, queryParams.PageNumber, queryParams.PageSize, cancellationToken);

            return new PagedResult<NotificationDto>
            {
                Items = notifications.Select(x => new NotificationDto
                {
                    Id = x.Id,
                    Title = x.Title,
                    Message = x.Message,
                    Type = x.Type,
                    IsRead = x.IsRead,
                    ReadDate = x.ReadDate,
                    Link = x.Link,
                    OrderId = x.OrderId,
                    CreatedDate = x.CreatedDate
                }).ToList(),
                TotalCount = totalCount
            };
        }

        public async Task<PagedResult<NotificationDto>> GetStaffNotificationsAsync(string userId, NotificationQueryParams queryParams, CancellationToken cancellationToken)
        {
            var totalCount = await _notificationRepository.CountForStaffAsync(userId, cancellationToken);

            if (totalCount == 0)
            {
                return new PagedResult<NotificationDto>
                {
                    Items = Array.Empty<NotificationDto>(),
                    TotalCount = 0
                };
            }

            var notifications = await _notificationRepository.GetStaffNotificationsAsync(userId, queryParams.PageNumber, queryParams.PageSize, cancellationToken);

            return new PagedResult<NotificationDto>
            {
                Items = notifications.Select(x => new NotificationDto
                {
                    Id = x.Id,
                    Title = x.Title,
                    Message = x.Message,
                    Type = x.Type,
                    IsRead = x.IsRead,
                    ReadDate = x.ReadDate,
                    Link = x.Link,
                    OrderId = x.OrderId,
                    CreatedDate = x.CreatedDate
                }).ToList(),
                TotalCount = totalCount
            };
        }

        public async Task<int> GetUnreadCountForUserAsync(string userId, CancellationToken cancellationToken)
        {
            return await _notificationRepository.GetUnreadCountForUserAsync(userId, cancellationToken);
        }

        public async Task<int> GetUnreadCountForStaffAsync(string userId, CancellationToken cancellationToken)
        {
            return await _notificationRepository.GetUnreadCountForStaffAsync(userId, cancellationToken);
        }

        public async Task MarkAsReadForUserAsync(long notificationId, string userId, CancellationToken cancellationToken)
        {
            var notification = await _notificationRepository.GetByIdForUserAsync(notificationId, userId, cancellationToken);

            if (notification is null)
                throw new NotFoundException("Notification not found.");

            if (notification.IsRead)
                return;

            notification.IsRead = true;
            notification.ReadDate = DateTime.UtcNow;

            await _unitOfWork.CommitAsync(cancellationToken);
        }

        public async Task MarkAsReadForStaffAsync(long notificationId, string userId, CancellationToken cancellationToken)
        {
            var notification = await _notificationRepository.GetByIdForStaffAsync(notificationId, userId, cancellationToken);

            if (notification is null)
                throw new NotFoundException("Notification not found.");

            if (notification.IsRead)
                return;

            notification.IsRead = true;
            notification.ReadDate = DateTime.UtcNow;

            await _unitOfWork.CommitAsync(cancellationToken);
        }
    }
}
