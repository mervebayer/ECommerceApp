using ECommerceApp.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApp.Domain.Entities
{
    public class PaymentTransaction : BaseEntity
    {
        public long OrderId { get; set; }
        public Order Order { get; set; } = null!;

        public PaymentTransactionStatus Status { get; set; }

        public string ConversationId { get; set; } = string.Empty;
        public string? ProviderPaymentId { get; set; }

        public string? ErrorCode { get; set; }
        public string? ErrorMessage { get; set; }
        public string IdempotencyKey { get; set; } = string.Empty;

    }
}
