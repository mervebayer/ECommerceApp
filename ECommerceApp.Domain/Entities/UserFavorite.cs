using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApp.Domain.Entities
{
    public class UserFavorite : BaseEntity
    {
        public string UserId { get; set; } = null!;
        public long ProductId { get; set; }

        public AppUser User { get; set; } = null!;
        public Product Product { get; set; } = null!;
    }
}
