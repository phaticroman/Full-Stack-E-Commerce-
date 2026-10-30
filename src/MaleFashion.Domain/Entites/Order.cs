using MaleFashion.Domain.Contracts;
using MaleFashion.Domain.Enums;
using MaleFashion.Domain.Exceptions;
using MaleFashion.Domain.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaleFashion.Domain.Entites
{
    public class Order : IAggregateRoot<Guid>
    {
        private const decimal FreeShippingThreshold = 100.00m;
        private const decimal FlatShippingFee = 5.00m;

        private readonly List<OrderItem> _items = new();

        public Guid Id { get; private set; }
        public Guid CustomerId { get; private set; }
        public DateTime OrderDate { get; private set; }
        public OrderStatus Status { get; private set; }
        public string ShippingFullName { get; private set; } = null!;
        public string ShippingPhone { get; private set; } = null!;
        public string ShippingEmail { get; private set; } = null!;
        public string ShippingCity { get; private set; } = null!;
        public string ShippingPostalCode { get; private set; } = null!;
        public decimal Subtotal { get; private set; }
        public decimal ShippingFee { get; private set; }
        public decimal DiscountAmount { get; private set; }
        public decimal Total { get; private set; }

        public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

        private Order() { } 

        public static Order Create(Guid customerId, string shippingFullName, string shippingPhone,
            string shippingEmail, string shippingCity, string shippingPostalCode)
        {
            if (customerId == Guid.Empty) throw new DomainException("CustomerId is required.");
            if (string.IsNullOrWhiteSpace(shippingFullName)) throw new DomainException("Shipping full name is required.");
            if (string.IsNullOrWhiteSpace(shippingPhone)) throw new DomainException("Shipping phone is required.");
            if (string.IsNullOrWhiteSpace(shippingEmail)) throw new DomainException("Shipping email is required.");
            if (string.IsNullOrWhiteSpace(shippingCity)) throw new DomainException("Shipping city is required.");
            if (string.IsNullOrWhiteSpace(shippingPostalCode)) throw new DomainException("Shipping postal code is required.");

            return new Order
            {
                Id = IdentityGenerator.NewSequentialGuid(),
                CustomerId = customerId,
                OrderDate = DateTime.UtcNow,
                Status = OrderStatus.Pending,
                ShippingFullName = shippingFullName.Trim(),
                ShippingPhone = shippingPhone.Trim(),
                ShippingEmail = shippingEmail.Trim(),
                ShippingCity = shippingCity.Trim(),
                ShippingPostalCode = shippingPostalCode.Trim(),
                DiscountAmount = 0m
            };
        }

        public void AddLineItem(Guid variantId, string productName, string size, string color, string sku,
            decimal unitPrice, int quantity)
        {
            if (Status != OrderStatus.Pending)
                throw new DomainException("Cannot add items to a non-pending order.");

            _items.Add(new OrderItem(IdentityGenerator.NewSequentialGuid(), Id, variantId, productName, size, color, sku, unitPrice, quantity));
            RecalculateTotals();
        }

        public void ConfirmPayment()
        {
            if (Status != OrderStatus.Pending)
                throw new DomainException($"Cannot confirm payment for an order with status '{Status}'.");
            Status = OrderStatus.Confirmed;
        }

        public void MarkShipped()
        {
            if (Status != OrderStatus.Confirmed)
                throw new DomainException("Only confirmed orders can be shipped.");
            Status = OrderStatus.Shipped;
        }

        public void MarkDelivered()
        {
            if (Status != OrderStatus.Shipped)
                throw new DomainException("Only shipped orders can be marked delivered.");
            Status = OrderStatus.Delivered;
        }

        public void Cancel()
        {
            if (Status is OrderStatus.Shipped or OrderStatus.Delivered or OrderStatus.Cancelled)
                throw new DomainException($"Cannot cancel order that is already {Status}.");

            Status = OrderStatus.Cancelled;
        }

        private void RecalculateTotals()
        {
            Subtotal = _items.Sum(i => i.UnitPrice * i.Quantity);
            ShippingFee = Subtotal >= FreeShippingThreshold || Subtotal == 0 ? 0m : FlatShippingFee;
            Total = (Subtotal - DiscountAmount) + ShippingFee;
        }
    }
}
