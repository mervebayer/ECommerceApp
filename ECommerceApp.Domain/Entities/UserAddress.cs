using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApp.Domain.Entities
{
    public class UserAddress : BaseEntity
    {
        public string UserId { get; set; } = null!;
        public string Title { get; set; } = null!;
        public string ContactName { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string Country { get; set; } = null!;
        public string City { get; set; } = null!;
        public string District { get; set; } = null!;
        public string PostalCode { get; set; } = null!;
        public string AddressLine { get; set; } = null!;
        public bool IsDefault { get; set; }
        public AppUser User { get; set; } = null!;

    }
}
