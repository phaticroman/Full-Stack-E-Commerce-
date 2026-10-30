using MaleFashion.Domain.Contracts;
using MaleFashion.Domain.Exceptions;
using MaleFashion.Domain.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaleFashion.Domain.Entites
{
    public class Product : IAggregateRoot<Guid>
    {
        private readonly List<ProductImage> _images = new();
        private readonly List<ProductVariant> _variants = new();

        public Guid Id { get; private set; }
        public string Name { get; private set; } = null!;
        public string Slug { get; private set; } = null!;
        public string? Description { get; private set; }
        public decimal Price { get; private set; }
        public decimal? DiscountPrice { get; private set; }
        public string Brand { get; private set; } = null!;
        public string ImageName { get; private set; } = null!;
        public bool IsActive { get; private set; } = true;
        public DateTime CreatedAt { get; private set; }
        public Guid CategoryId { get; private set; }
        public Category? Category { get; private set; }

        public IReadOnlyCollection<ProductImage> Images => _images.AsReadOnly();
        public IReadOnlyCollection<ProductVariant> Variants => _variants.AsReadOnly();

        private Product() { } 

        public static Product Create(
            string name, string slug, string? description, decimal price, decimal? discountPrice,
            string brand, string imageName, Guid categoryId, DateTime createdAtUtc, bool isActive = true)
        {
            ValidatePricing(name, price, discountPrice);

            return new Product
            {
                Id = IdentityGenerator.NewSequentialGuid(),
                Name = name.Trim(),
                Slug = slug.Trim().ToLowerInvariant(),
                Description = description?.Trim(),
                Price = price,
                DiscountPrice = discountPrice,
                Brand = brand.Trim(),
                ImageName = imageName,
                CategoryId = categoryId,
                CreatedAt = createdAtUtc,
                IsActive = isActive
            };
        }

        public void UpdateDetails(string name, string slug, string? description, decimal price,
            decimal? discountPrice, string brand, Guid categoryId)
        {
            ValidatePricing(name, price, discountPrice);

            Name = name.Trim();
            Slug = slug.Trim().ToLowerInvariant();
            Description = description?.Trim();
            Price = price;
            DiscountPrice = discountPrice;
            Brand = brand.Trim();
            CategoryId = categoryId;
        }

        public void ChangeMainImage(string imageName) => ImageName = imageName;
        public void Activate() => IsActive = true;
        public void Deactivate() => IsActive = false;

        public ProductVariant AddVariant(string size, string color, string sku, int stockQuantity)
        {
            if (_variants.Any(v => v.Size.Equals(size, StringComparison.OrdinalIgnoreCase) &&
                                   v.Color.Equals(color, StringComparison.OrdinalIgnoreCase)))
                throw new DomainException("This size/color combination already exists for this product.");

            var variant = new ProductVariant(IdentityGenerator.NewSequentialGuid(), Id, size, color, sku, stockQuantity);
            _variants.Add(variant);
            return variant;
        }
        public void UpdateVariant(Guid variantId, string size, string color, string sku, int stockQuantity)
        {
            var variant = GetVariant(variantId);

            if (_variants.Any(v => v.Id != variantId &&
                                   v.Size.Equals(size, StringComparison.OrdinalIgnoreCase) &&
                                   v.Color.Equals(color, StringComparison.OrdinalIgnoreCase)))
                throw new DomainException("This size/color combination already exists for this product.");

            variant.UpdateDetails(size, color, sku, stockQuantity);
        }
        public void RemoveVariant(Guid variantId)
        {
            var variant = _variants.FirstOrDefault(v => v.Id == variantId)
                ?? throw new DomainException("Variant not found on this product.");
            _variants.Remove(variant);
        }

        public ProductVariant GetVariant(Guid variantId) =>
            _variants.FirstOrDefault(v => v.Id == variantId)
                ?? throw new DomainException("Variant not found on this product.");

        public ProductImage AddImage(string imageName)
        {
            var nextOrder = _images.Count == 0 ? 1 : _images.Max(i => i.DisplayOrder) + 1;
            var image = new ProductImage(IdentityGenerator.NewSequentialGuid(), Id, imageName, nextOrder);
            _images.Add(image);
            return image;
        }

        public void RemoveImage(Guid imageId)
        {
            var image = _images.FirstOrDefault(i => i.Id == imageId)
                ?? throw new DomainException("Image not found on this product.");
            _images.Remove(image);
        }

        public void ReorderImage(Guid imageId, int displayOrder)
        {
            var image = _images.FirstOrDefault(i => i.Id == imageId)
                ?? throw new DomainException("Image not found on this product.");
            image.SetDisplayOrder(displayOrder);
        }

        public decimal GetEffectivePrice() => DiscountPrice ?? Price;

        private static void ValidatePricing(string name, decimal price, decimal? discountPrice)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("Product name is required.");
            if (price <= 0)
                throw new DomainException("Price must be greater than zero.");
            if (discountPrice.HasValue && discountPrice.Value >= price)
                throw new DomainException("Discount price must be lower than the regular price.");
        }
    }
}
