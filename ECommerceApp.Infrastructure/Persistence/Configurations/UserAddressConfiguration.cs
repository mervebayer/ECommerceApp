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
    public class UserAddressConfiguration : IEntityTypeConfiguration<UserAddress>
    {
        public void Configure(EntityTypeBuilder<UserAddress> builder)
        {
            builder.ToTable("UserAddresses");
           
            builder.HasKey(x => x.Id);
                     
            builder.Property(x => x.Title)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x => x.ContactName)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(x => x.PhoneNumber)
                .IsRequired()
                .HasMaxLength(15);

            builder.Property(x => x.Country)
                .IsRequired()
                .HasMaxLength(60);

            builder.Property(x => x.City)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.District)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.PostalCode)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(x => x.AddressLine)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(x => x.IsDefault)
              .IsRequired();

            builder.HasIndex(x => x.UserId)
                .HasDatabaseName("IX_UserAddresses_UserId");

            builder.HasIndex(x => new { x.UserId, x.IsDefault })
                    .IsUnique()
                    .HasFilter("[IsDefault] = 1");

            builder.HasOne(x => x.User)
                .WithMany(x => x.Addresses)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
