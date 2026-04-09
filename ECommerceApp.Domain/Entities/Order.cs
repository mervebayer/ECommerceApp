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
        public string OrderNumber { get; set; } = null!;
        public decimal TotalAmount { get; set; }
        public OrderStatus Status { get; set; } = OrderStatus.PendingPayment;
        public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
        public string ShippingTitle { get; set; } = null!;
        public string ShippingContactName { get; set; } = null!;
        public string ShippingPhoneNumber { get; set; } = null!;
        public string ShippingCountry { get; set; } = null!;
        public string ShippingCity { get; set; } = null!;
        public string ShippingDistrict { get; set; } = null!;
        public string ShippingPostalCode { get; set; } = null!;
        public string ShippingAddressLine { get; set; } = null!;
        public DateTime? ReservationExpiresAt { get; set; }
        //public string BasketId { get; set; } = null!;
        public string? BasketId { get; set; }
        public ICollection<PaymentTransaction> PaymentTransactions { get; set; } = new List<PaymentTransaction>();

    }
}
