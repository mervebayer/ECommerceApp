using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApp.Infrastructure.Payments.Iyzico.Models
{
    public sealed class CreatePaymentResponse
    {
        //public string Status { get; set; } = string.Empty;
        //public string ConversationId { get; set; } = string.Empty;
        //public string PaymentId { get; set; } = string.Empty;
        //public string ErrorCode { get; set; } = string.Empty;
        //public string ErrorMessage { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;
        public string Locale { get; set; } = string.Empty;
        public long SystemTime { get; set; }
        public string ConversationId { get; set; } = string.Empty;
        public string PaymentId { get; set; } = string.Empty;
        public string Price { get; set; } = string.Empty;
        public string PaidPrice { get; set; } = string.Empty;
        public string Currency { get; set; } = string.Empty;
        public string BasketId { get; set; } = string.Empty;
        public string ErrorCode { get; set; } = string.Empty;
        public string ErrorMessage { get; set; } = string.Empty;
        public string ErrorGroup { get; set; } = string.Empty;
    }
}
