using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApp.Application.DTOs.Payments
{
    public sealed record PaymentCardDto{
        public string CardHolderName { get; init; } = null!;
        public string CardNumber { get; init; } = null!;
        public string ExpireMonth { get; init; } = null!;
        public string ExpireYear { get; init; } = null!;
        public string Cvc { get; init; } = null!;
    }
}
