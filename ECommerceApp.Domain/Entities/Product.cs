using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApp.Domain.Entities
{
    public class Product : BaseEntity
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set;}
        public long CategoryId { get; set; }
        public Category Category { get; set; }
        public ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();
        public ICollection<UserFavorite> Favorited {get; set; } = new List<UserFavorite>();

    }
}
