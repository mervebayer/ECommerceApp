using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApp.Application.DTOs.Payments
{
    public sealed record ProcessPaymentRequest
    {
        public string ConversationId { get; init; } = string.Empty;
        public string BasketId { get; init; } = string.Empty;
        public decimal Price { get; init; }
        public decimal PaidPrice { get; init; }

        public PaymentCardDto PaymentCard { get; init; } = null!;

        public string BuyerId { get; init; } = string.Empty;
        public string BuyerIp { get; init; } = string.Empty;
        public string BuyerEmail { get; init; } = string.Empty;
        public string BuyerFirstName { get; init; } = string.Empty;
        public string BuyerLastName { get; init; } = string.Empty;
        public string BuyerPhoneNumber { get; init; } = string.Empty;
        public string BuyerIdentityNumber { get; init; } = string.Empty;

        public string ShippingContactName { get; init; } = string.Empty;
        public string AddressLine { get; init; } = string.Empty;
        public string City { get; init; } = string.Empty;
        public string Country { get; init; } = string.Empty;
        public string ZipCode { get; init; } = string.Empty;

        public IReadOnlyList<PaymentBasketItemDto> Items { get; init; } = [];
    }
}
