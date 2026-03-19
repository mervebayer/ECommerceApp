using ECommerceApp.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApp.Domain.Entities
{
    public class Order : BaseEntity
    {
        // Non-nullable property initialized by EF Core at runtime (null-forgiving used to suppress compiler warning)
        public string UserId { get; set; } = default!;
        public decimal TotalAmount { get; set; }
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();

    }
}
