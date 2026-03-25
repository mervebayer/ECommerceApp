using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApp.Application.DTOs.Orders
{
    public sealed record OrderListDto(long OrderId, string OrderNumber, decimal TotalAmount, string Status, int ItemCount, DateTime CreatedDate);

}
