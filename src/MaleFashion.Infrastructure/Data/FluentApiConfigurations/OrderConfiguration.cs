using MaleFashion.Domain.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaleFashion.Infrastructure.Data.FluentApiConfigurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.ToTable("Orders");

            builder.HasKey(o => o.Id);
            builder.Property(o => o.Id).ValueGeneratedNever();

            builder.Property(o => o.CustomerId).IsRequired();
            builder.Property(o => o.OrderDate).IsRequired();

            builder.Property(o => o.Status)
                   .IsRequired()
                   .HasConversion<string>()
                   .HasMaxLength(20);

            builder.Property(o => o.ShippingFullName).IsRequired().HasMaxLength(200);
            builder.Property(o => o.ShippingPhone).IsRequired().HasMaxLength(20);
            builder.Property(o => o.ShippingEmail).IsRequired().HasMaxLength(256);
            builder.Property(o => o.ShippingCity).IsRequired().HasMaxLength(100);
            builder.Property(o => o.ShippingPostalCode).IsRequired().HasMaxLength(20);

            builder.Property(o => o.Subtotal).IsRequired().HasColumnType("decimal(18,2)");
            builder.Property(o => o.ShippingFee).IsRequired().HasColumnType("decimal(18,2)");
            builder.Property(o => o.DiscountAmount).IsRequired().HasColumnType("decimal(18,2)");
            builder.Property(o => o.Total).IsRequired().HasColumnType("decimal(18,2)");

            builder.HasIndex(o => o.CustomerId);
            builder.HasIndex(o => o.OrderDate);

            builder.HasMany(o => o.Items)
                .WithOne(i => i.Order)
                .HasForeignKey(i => i.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Navigation(o => o.Items)
                   .UsePropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}
