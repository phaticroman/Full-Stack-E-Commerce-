using MaleFashion.Domain.Contracts;
using MaleFashion.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaleFashion.Domain.Entites
{
    public class OrderItem
    {
        public Guid Id { get; private set; }
        public Guid OrderId { get; private set; }
        public Order? Order { get; private set; }
        public Guid? ProductVariantId { get; private set; }
        public ProductVariant? ProductVariant { get; private set; }
        public string ProductName { get; private set; } = null!;
        public string Size { get; private set; } = null!;
        public string Color { get; private set; } = null!;
        public string SKU { get; private set; } = null!;
        public decimal UnitPrice { get; private set; }
        public int Quantity { get; private set; }

        private OrderItem() { } 

        internal OrderItem(Guid id, Guid orderId, Guid variantId, string productName, string size, string color,
            string sku, decimal unitPrice, int quantity)
        {
            if (quantity <= 0) throw new DomainException("Quantity must be greater than zero.");
            if (unitPrice < 0) throw new DomainException("Unit price cannot be negative.");

            Id = id;
            OrderId = orderId;
            ProductVariantId = variantId;
            ProductName = productName;
            Size = size;
            Color = color;
            SKU = sku;
            UnitPrice = unitPrice;
            Quantity = quantity;
        }
    }
}
