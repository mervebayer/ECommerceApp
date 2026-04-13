using ECommerceApp.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApp.Domain.Entities
{
    public class Notification : BaseEntity
    {
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public NotificationType Type { get; set; }
        public NotificationAudience Audience { get; set; }
        public bool IsRead { get; set; } = false;
        public DateTime? ReadDate { get; set; }
        public string? ReceiverUserId { get; set; }
        public string? Link { get; set; }
        public long? OrderId { get; set; }

        public Order? Order { get; set; }
    }
}
