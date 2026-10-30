using MaleFashion.Domain.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaleFashion.Infrastructure.Data.FluentApiConfigurations
{
    public class CartItemConfiguration : IEntityTypeConfiguration<CartItem>
    {
        public void Configure(EntityTypeBuilder<CartItem> builder)
        {
            builder.ToTable("CartItems");

            builder.HasKey(ci => ci.Id);
            builder.Property(ci => ci.Id).ValueGeneratedNever();

            builder.Property(ci => ci.Quantity).IsRequired();

            builder.ToTable(t => t.HasCheckConstraint("CK_CartItem_Quantity", "[Quantity] > 0"));

            builder.HasIndex(ci => new { ci.CartId, ci.ProductVariantId }).IsUnique();

            builder.HasOne(ci => ci.ProductVariant)
                .WithMany()
                .HasForeignKey(ci => ci.ProductVariantId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
