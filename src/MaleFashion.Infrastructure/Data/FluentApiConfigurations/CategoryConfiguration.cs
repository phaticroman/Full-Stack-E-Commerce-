using MaleFashion.Domain.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaleFashion.Infrastructure.Data.FluentApiConfigurations
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.ToTable("Categories");

            builder.HasKey(c => c.Id);
            builder.Property(c => c.Id).ValueGeneratedNever();

            builder.Property(c => c.Name).IsRequired().HasMaxLength(200);
            builder.Property(c => c.Slug).IsRequired().HasMaxLength(250);
            builder.Property(c => c.Description).HasMaxLength(1000);
            builder.Property(c => c.ImageName).HasMaxLength(500);
            builder.Property(c => c.IsActive).IsRequired();
            builder.Property(c => c.DisplayOrder).IsRequired();

            builder.HasIndex(c => c.Slug).IsUnique();
        }
    }
}
