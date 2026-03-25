using ECommerceApp.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApp.Application.Common.Helpers
{
    public class OrderNumberGenerator : IOrderNumberGenerator
    {
        public string Generate()
        {
            var date = DateTime.UtcNow.ToString("yyyyMMdd");

            var random = Guid.NewGuid().ToString("N")[..6].ToUpper();

            return $"ORD-{date}-{random}";
        }
    }
}
