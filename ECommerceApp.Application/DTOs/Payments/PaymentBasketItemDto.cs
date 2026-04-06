using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApp.Application.DTOs.Payments
{
    public sealed record PaymentBasketItemDto
    {
        public string Id { get; init; } = null!;
        public string Name { get; init; } = null!;
        public string Category1 { get; init; } = null!;
        public string ItemType { get; init; } = "PHYSICAL";
        public decimal Price { get; init; }
    }
}
