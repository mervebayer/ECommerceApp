using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApp.Application.DTOs.Products
{
    public class ProductDescriptionGenerationInput
    {
        public string Name { get; set; } = string.Empty;
        public string? CurrentDescription { get; set; }
        public string? CategoryName { get; set; }
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
    }
}
