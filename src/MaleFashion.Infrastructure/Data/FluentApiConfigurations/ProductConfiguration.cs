using MaleFashion.Domain.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaleFashion.Infrastructure.Data.FluentApiConfigurations
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("Products");

            builder.HasKey(p => p.Id);
            builder.Property(p => p.Id).ValueGeneratedNever();

            builder.Property(p => p.Name).IsRequired().HasMaxLength(200);
            builder.Property(p => p.Slug).IsRequired().HasMaxLength(250);
            builder.Property(p => p.Description).HasMaxLength(2000);
            builder.Property(p => p.Price).IsRequired().HasColumnType("decimal(18,2)");
            builder.Property(p => p.DiscountPrice).HasColumnType("decimal(18,2)");
            builder.Property(p => p.Brand).IsRequired().HasMaxLength(100);
            builder.Property(p => p.ImageName).IsRequired().HasMaxLength(500);
            builder.Property(p => p.IsActive).IsRequired();
            builder.Property(p => p.CreatedAt).IsRequired();

            builder.HasIndex(p => p.Slug).IsUnique();
            builder.HasIndex(p => p.CategoryId);

            builder.HasOne(p => p.Category)
                .WithMany()
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(p => p.Images)
                .WithOne(i => i.Product)
                .HasForeignKey(i => i.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(p => p.Variants)
                .WithOne(v => v.Product)
                .HasForeignKey(v => v.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Navigation(p => p.Images)
                   .UsePropertyAccessMode(PropertyAccessMode.Field);

            builder.Navigation(p => p.Variants)
                   .UsePropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}
