using MaleFashion.Domain.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaleFashion.Infrastructure.Data.FluentApiConfigurations
{
    public class ProductVariantConfiguration : IEntityTypeConfiguration<ProductVariant>
    {
        public void Configure(EntityTypeBuilder<ProductVariant> builder)
        {
            builder.ToTable("ProductVariants");

            builder.HasKey(v => v.Id);
            builder.Property(v => v.Id).ValueGeneratedNever();

            builder.Property(v => v.Size).IsRequired().HasMaxLength(50);
            builder.Property(v => v.Color).IsRequired().HasMaxLength(50);
            builder.Property(v => v.SKU).IsRequired().HasMaxLength(100);
            builder.Property(v => v.StockQuantity).IsRequired();

            builder.HasIndex(v => v.SKU).IsUnique();
            builder.HasIndex(v => new { v.ProductId, v.Size, v.Color }).IsUnique();
        }
    }
}
