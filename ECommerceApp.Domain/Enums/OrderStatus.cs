using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApp.Domain.Enums
{
    public enum OrderStatus
    {
        PendingPayment = 1,
        Confirmed = 2,
        Preparing = 3,
        Shipped = 4,
        Delivered = 5,
        Completed = 6,
        Cancelled = 7,
        Expired = 8,
        DeliveryFailed = 9
    }
}
