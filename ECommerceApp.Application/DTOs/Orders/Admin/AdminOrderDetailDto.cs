using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApp.Application.DTOs.Orders.Admin
{
    public sealed record AdminOrderDetailDto(long OrderId, string UserId, string OrderNumber, decimal TotalAmount, string Status, DateTime CreatedDate, IReadOnlyList<OrderItemDto> Items, string ShippingTitle,
        string ShippingContactName,
        string ShippingPhoneNumber,
        string ShippingCountry,
        string ShippingCity,
        string ShippingDistrict,
        string ShippingPostalCode,
        string ShippingAddressLine);
}
