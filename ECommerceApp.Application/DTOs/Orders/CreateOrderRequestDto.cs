using ECommerceApp.Application.DTOs.Payments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApp.Application.DTOs.Orders
{

    public sealed record CreateOrderRequestDto
    {
        public long UserAddressId { get; init; }
        public PaymentCardDto PaymentCard { get; init; }
        //public string? ShippingAddress { get; init; }
        //public string? PaymentMethod { get; init; }
    }
}
