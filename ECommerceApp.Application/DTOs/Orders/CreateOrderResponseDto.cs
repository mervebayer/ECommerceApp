using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApp.Application.DTOs.Orders
{
    public sealed record CreateOrderResponseDto(string OrderNumber, decimal TotalAmount, string Status, int ItemCount);
}
