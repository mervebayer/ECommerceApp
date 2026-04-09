using ECommerceApp.Domain.Entities;
using ECommerceApp.Domain.Enums;
using ECommerceApp.Domain.Interfaces.Repositories;
using ECommerceApp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApp.Infrastructure.Repositories
{
    public class PaymentTransactionRepository : GenericRepository<PaymentTransaction>, IPaymentTransactionRepository
    {
        private readonly AppDbContext _context;

        public PaymentTransactionRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<bool> HasPendingTransactionAsync(long orderId, DateTime thresholdUtc, CancellationToken cancellationToken = default)
        {
            return await _context.PaymentTransactions.AsNoTracking().AnyAsync(x => x.OrderId == orderId && x.Status == PaymentTransactionStatus.Pending && x.CreatedDate >= thresholdUtc, cancellationToken);
        }

        public async Task<bool> HasSuccessfulTransactionAsync(long orderId, CancellationToken cancellationToken = default)
        {
            return await _context.PaymentTransactions.AsNoTracking().AnyAsync(x => x.OrderId == orderId && x.Status == PaymentTransactionStatus.Succeeded, cancellationToken);
        }

        public async Task<IReadOnlyList<PaymentTransaction>> GetByOrderIdAsync(long orderId, CancellationToken cancellationToken = default)
        {
            return await _context.PaymentTransactions.AsNoTracking().Where(x => x.OrderId == orderId).OrderByDescending(x => x.CreatedDate).ToListAsync(cancellationToken);
        }

        public async Task<PaymentTransaction?> GetLatestByOrderIdAsync(long orderId, CancellationToken cancellationToken = default)
        {
            return await _context.PaymentTransactions.AsNoTracking().Where(x => x.OrderId == orderId).OrderByDescending(x => x.CreatedDate).FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<PaymentTransaction>> GetExpiredPendingTransactionsAsync(long orderId, DateTime thresholdUtc, CancellationToken cancellationToken = default)
        {
            return await _context.PaymentTransactions.Where(x => x.OrderId == orderId && x.Status == PaymentTransactionStatus.Pending && x.CreatedDate < thresholdUtc).OrderBy(x => x.CreatedDate).ToListAsync(cancellationToken);
        }

        public async Task<PaymentTransaction?> GetByOrderIdAndIdempotencyKeyAsync(long orderId, string idempotencyKey, CancellationToken cancellationToken = default)
        {
            return await _context.PaymentTransactions.AsNoTracking().FirstOrDefaultAsync(x => x.OrderId == orderId && x.IdempotencyKey == idempotencyKey, cancellationToken);
        }


    }
}
