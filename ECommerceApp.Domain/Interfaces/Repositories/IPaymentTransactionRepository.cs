using ECommerceApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApp.Domain.Interfaces.Repositories
{
    public interface IPaymentTransactionRepository : IGenericRepository<PaymentTransaction>
    {
        Task<bool> HasPendingTransactionAsync(long orderId, DateTime thresholdUtc, CancellationToken cancellationToken = default);
        Task<bool> HasSuccessfulTransactionAsync(long orderId, CancellationToken cancellationToken = default);
        Task<PaymentTransaction?> GetLatestByOrderIdAsync(long orderId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<PaymentTransaction>> GetByOrderIdAsync(long orderId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<PaymentTransaction>> GetExpiredPendingTransactionsAsync(long orderId, DateTime thresholdUtc, CancellationToken cancellationToken = default);
        Task<PaymentTransaction?> GetByOrderIdAndIdempotencyKeyAsync(long orderId, string idempotencyKey, CancellationToken cancellationToken = default);

    }
}
