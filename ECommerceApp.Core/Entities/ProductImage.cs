using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApp.Core.Entities
{
    public class ProductImage : BaseEntity
    {
        public long ProductId { get; set; }
        public Product Product { get; set; }
        public string Url { get; set; }
        public string PublicId { get; set; }
        public bool IsMain { get; set; }
    }
}
