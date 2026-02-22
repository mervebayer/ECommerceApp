using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApp.Core.DTOs.Baskets
{
    public class BasketItemUpsertDto
    {
        public long ProductId { get; set; }
        public int Quantity { get; set; } = 1;
    }
}
