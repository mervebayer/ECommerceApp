using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApp.Application.DTOs.Payments
{
    public sealed record PayOrderRequestDto
    {
        public PaymentCardDto PaymentCard { get; init; } = null!;
    }

}
