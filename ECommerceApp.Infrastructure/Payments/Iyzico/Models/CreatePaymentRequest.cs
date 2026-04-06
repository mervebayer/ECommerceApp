using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApp.Infrastructure.Payments.Iyzico.Models
{
    public sealed class CreatePaymentRequest
    {
        public string ConversationId { get; set; } = string.Empty;
        public string Locale { get; set; } = "tr";
        public decimal Price { get; set; }
        public decimal PaidPrice { get; set; }
        public string PaymentGroup { get; set; } = "PRODUCT";
        public string Currency { get; set; } = "TRY";
        public int Installment { get; set; } = 1;
        public string BasketId { get; set; } = string.Empty;
        public string PaymentChannel { get; set; } = "WEB";
        public IyzicoPaymentCard PaymentCard { get; set; } = new();
        public IyzicoBuyer Buyer { get; set; } = new();
        public IyzicoAddress ShippingAddress { get; set; } = new();
        public IyzicoAddress BillingAddress { get; set; } = new();
        public List<IyzicoBasketItem> BasketItems { get; set; } = [];
    }
}
