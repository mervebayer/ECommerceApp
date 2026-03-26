using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApp.Application.DTOs.Addresses
{
    public class UserAddressDto
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string ContactName { get; set; } 
        public string PhoneNumber { get; set; }
        public string Country { get; set; }
        public string City { get; set; } 
        public string District { get; set; }
        public string PostalCode { get; set; } 
        public string AddressLine { get; set; }
        public bool IsDefault { get; set; }
    }
}
