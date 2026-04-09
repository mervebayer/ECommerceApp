using ECommerceApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApp.Infrastructure.Persistence.Configurations
{
    public class PaymentTransactionConfiguration : IEntityTypeConfiguration<PaymentTransaction>
    {
        public void Configure(EntityTypeBuilder<PaymentTransaction> builder)
        {
            builder.ToTable("PaymentTransactions");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Status)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(x => x.ConversationId)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.ProviderPaymentId)
                .HasMaxLength(200);

            builder.Property(x => x.ErrorCode)
                .HasMaxLength(100);

            builder.Property(x => x.ErrorMessage)
                .HasMaxLength(1000);

            builder.HasOne(x => x.Order)
                .WithMany(x => x.PaymentTransactions)
                .HasForeignKey(x => x.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => x.OrderId);
            builder.HasIndex(x => x.ConversationId);

            builder.Property(x => x.IdempotencyKey)
                .IsRequired()
                .HasMaxLength(100);

            builder.HasIndex(x => new { x.OrderId, x.IdempotencyKey })
                .IsUnique();

        }
    }
}
