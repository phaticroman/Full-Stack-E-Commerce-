using MaleFashion.Domain.Contracts;
using MaleFashion.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaleFashion.Domain.Entites
{
    public class ProductVariant
    {
        public Guid Id { get; private set; }
        public Guid ProductId { get; private set; }
        public Product? Product { get; private set; }
        public string Size { get; private set; } = null!;
        public string Color { get; private set; } = null!;
        public string SKU { get; private set; } = null!;
        public int StockQuantity { get; private set; }

        private ProductVariant() { } 

        internal ProductVariant(Guid id, Guid productId, string size, string color, string sku, int stockQuantity)
        {
            if (string.IsNullOrWhiteSpace(size)) throw new DomainException("Variant size is required.");
            if (string.IsNullOrWhiteSpace(color)) throw new DomainException("Variant color is required.");
            if (string.IsNullOrWhiteSpace(sku)) throw new DomainException("Variant SKU is required.");
            if (stockQuantity < 0) throw new DomainException("Stock cannot be negative.");

            Id = id;
            ProductId = productId;
            Size = size.Trim();
            Color = color.Trim();
            SKU = sku.Trim().ToUpperInvariant();
            StockQuantity = stockQuantity;
        }

        public void DeductStock(int quantity)
        {
            if (quantity <= 0)
                throw new DomainException("Quantity to deduct must be greater than zero.");
            if (StockQuantity < quantity)
                throw new DomainException($"Insufficient stock for SKU '{SKU}'. Only {StockQuantity} available.");

            StockQuantity -= quantity;
        }

        public void Restock(int quantity)
        {
            if (quantity <= 0)
                throw new DomainException("Quantity to restock must be greater than zero.");

            StockQuantity += quantity;
        }

        internal void UpdateDetails(string size, string color, string sku, int stockQuantity)
        {
            if (string.IsNullOrWhiteSpace(size)) throw new DomainException("Variant size is required.");
            if (string.IsNullOrWhiteSpace(color)) throw new DomainException("Variant color is required.");
            if (string.IsNullOrWhiteSpace(sku)) throw new DomainException("Variant SKU is required.");
            if (stockQuantity < 0) throw new DomainException("Stock cannot be negative.");

            Size = size.Trim();
            Color = color.Trim();
            SKU = sku.Trim().ToUpperInvariant();
            StockQuantity = stockQuantity;
        }
    }
}
