using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApp.Application.DTOs.Orders.Admin
{
    public sealed record AdminOrderListDto(long OrderId, string OrderNumber, string UserId, decimal TotalAmount, string Status, int ItemCount, DateTime CreatedDate);
    
}
