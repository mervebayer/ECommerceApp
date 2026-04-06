using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApp.Infrastructure.Payments.Iyzico.Models
{
    public sealed class IyzicoBasketItem
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Category1 { get; set; } = string.Empty;
        public string ItemType { get; set; } = "PHYSICAL";
        public decimal Price { get; set; }
    }
}
