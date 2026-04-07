using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApp.Application.DTOs.Payments
{
    public sealed record PayOrderResponseDto(long OrderId, string OrderNumber, string Status, bool IsPaymentSuccessful, string? PaymentErrorMessage);

}
