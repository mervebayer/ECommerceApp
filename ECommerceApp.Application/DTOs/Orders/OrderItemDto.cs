using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApp.Application.DTOs.Orders
{
    public sealed record OrderItemDto(long ProductId ,string ProductName, decimal UnitPrice, int Quantity, decimal LineTotal);
}
