using ECommerceApp.Application.DTOs.Payments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApp.Application.Interfaces
{
    public interface IPaymentGateway
    {
        Task<PaymentResult> ProcessAsync(ProcessPaymentRequest request, CancellationToken cancellationToken = default); 
    }
}
