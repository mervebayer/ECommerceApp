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
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.ToTable("Categories");

            builder.HasKey(x => x.Id);
           
            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(50);


            //builder.HasMany(x => x.Products)
            //    .WithOne(x => x.Category)
            //    .HasForeignKey(x => x.CategoryId)
            //    .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
