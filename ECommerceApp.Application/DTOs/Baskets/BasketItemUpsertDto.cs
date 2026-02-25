using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApp.Application.DTOs.Baskets
{
    public class BasketItemUpsertDto
    {
        public long ProductId { get; set; }
        public int Quantity { get; set; } = 1;
    }
}
