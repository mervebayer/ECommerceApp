using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApp.Application.DTOs.Payments
{
    public sealed record PaymentResult
{
    public bool IsSuccess { get; init; }
    public string? PaymentId { get; init; }
    public string? ConversationId { get; init; }
    public string? ErrorCode { get; init; }
    public string? ErrorMessage { get; init; }
}
}
