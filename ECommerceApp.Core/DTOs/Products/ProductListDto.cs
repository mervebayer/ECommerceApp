using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApp.Core.DTOs.Products
{
    public class ProductListDto : BaseDto
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public long CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string? MainImageUrl { get; set; }
    }
}
