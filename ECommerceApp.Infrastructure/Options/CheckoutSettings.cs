using ECommerceApp.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApp.Infrastructure.Options
{
    public class CheckoutSettings : ICheckoutSettings
    {
        public int ReservationTimeoutMinutes { get; set; }
    }

}
