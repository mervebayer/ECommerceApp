using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApp.Core.DTOs
{
    public abstract class BaseDto
    {
        public long  Id { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
