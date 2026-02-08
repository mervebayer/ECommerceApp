using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApp.Core.Entities
{
    public abstract class BaseEntity
    {
        public long Id { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedDate { get; set; } 
        public bool IsDeleted { get; set; }
    }
}
