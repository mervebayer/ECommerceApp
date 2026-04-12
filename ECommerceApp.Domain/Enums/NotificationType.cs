using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApp.Domain.Enums
{
    public enum NotificationType
    {
        OrderCreated = 1,
        Campaign = 2,
        OrderStatusChanged = 3,
        PaymentReceived = 4,
        OrderCancelled = 5,
        StockWarning = 6

    }
}
