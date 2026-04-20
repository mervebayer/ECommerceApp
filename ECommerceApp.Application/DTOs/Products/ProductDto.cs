using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApp.Application.DTOs.Products
{
    public class ProductDto : BaseDto
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public long CategoryId { get; set; }
        public string CategoryName { get; set; }
        public ICollection<ProductImageDto> Images { get; set; } = new List<ProductImageDto>();

    }
}
