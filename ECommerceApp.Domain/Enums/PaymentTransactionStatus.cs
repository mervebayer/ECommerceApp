using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApp.Domain.Enums
{
    public enum PaymentTransactionStatus
    {
        Pending = 1,
        Succeeded = 2,
        Failed = 3,
        Expired = 4
    }
}
