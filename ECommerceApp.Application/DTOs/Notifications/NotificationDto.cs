using ECommerceApp.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApp.Application.DTOs.Notifications
{
    public class NotificationDto
    {
        public long Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;

        public NotificationType Type { get; set; }

        public bool IsRead { get; set; }

        public DateTime? ReadDate { get; set; }

        public string? Link { get; set; }

        public long? OrderId { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
