using MaleFashion.Domain.Contracts;
using MaleFashion.Domain.Exceptions;
using MaleFashion.Domain.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaleFashion.Domain.Entites
{
    public class Cart : IAggregateRoot<Guid>
    {
        private readonly List<CartItem> _items = new();

        public Guid Id { get; private set; }
        public Guid CustomerId { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }

        public IReadOnlyCollection<CartItem> Items => _items.AsReadOnly();
        public bool IsEmpty => _items.Count == 0;

        private Cart() { }

        public static Cart StartFor(Guid customerId, DateTime createdAtUtc)
        {
            if (customerId == Guid.Empty)
                throw new DomainException("Customer id is required to start a cart.");

            return new Cart
            {
                Id = IdentityGenerator.NewSequentialGuid(),
                CustomerId = customerId,
                CreatedAt = createdAtUtc
            };
        }

        public void AddItem(Guid productVariantId, int quantity)
        {
            if (quantity <= 0)
                throw new DomainException("Quantity must be greater than zero.");

            var existing = _items.FirstOrDefault(i => i.ProductVariantId == productVariantId);
            if (existing != null)
                existing.IncreaseQuantity(quantity);
            else
                _items.Add(new CartItem(IdentityGenerator.NewSequentialGuid(), Id, productVariantId, quantity));

            Touch();
        }

        public void RemoveItem(Guid cartItemId)
        {
            var item = _items.FirstOrDefault(i => i.Id == cartItemId)
                ?? throw new DomainException("Cart item not found.");
            _items.Remove(item);
            Touch();
        }

        public void UpdateItemQuantity(Guid cartItemId, int quantity)
        {
            if (quantity <= 0)
                throw new DomainException("Quantity must be greater than zero.");

            var item = _items.FirstOrDefault(i => i.Id == cartItemId)
                ?? throw new DomainException("Cart item not found.");
            item.SetQuantity(quantity);
            Touch();
        }

        public void Clear()
        {
            _items.Clear();
            Touch();
        }

        private void Touch() => UpdatedAt = DateTime.UtcNow;
    }

}
