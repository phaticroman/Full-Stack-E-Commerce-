using MaleFashion.Domain.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaleFashion.Infrastructure.Data.FluentApiConfigurations
{
    public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            builder.ToTable("OrderItems");

            builder.HasKey(oi => oi.Id);
            builder.Property(oi => oi.Id).ValueGeneratedNever();

            builder.Property(oi => oi.ProductName).IsRequired().HasMaxLength(200);
            builder.Property(oi => oi.Size).IsRequired().HasMaxLength(50);
            builder.Property(oi => oi.Color).IsRequired().HasMaxLength(50);
            builder.Property(oi => oi.SKU).IsRequired().HasMaxLength(100);
            builder.Property(oi => oi.UnitPrice).IsRequired().HasColumnType("decimal(18,2)");
            builder.Property(oi => oi.Quantity).IsRequired();

            builder.HasOne(oi => oi.ProductVariant)
                .WithMany()
                .HasForeignKey(oi => oi.ProductVariantId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
