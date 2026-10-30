using MaleFashion.Domain.Contracts;
using MaleFashion.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaleFashion.Domain.Entites
{
    public class CartItem
    {
        public Guid Id { get; private set; }
        public Guid CartId { get; private set; }
        public Cart? Cart { get; private set; }
        public Guid ProductVariantId { get; private set; }
        public ProductVariant? ProductVariant { get; private set; }
        public int Quantity { get; private set; }

        private CartItem() { } 

        internal CartItem(Guid id, Guid cartId, Guid productVariantId, int quantity)
        {
            if (quantity <= 0)
                throw new DomainException("Quantity must be greater than zero.");

            Id = id;
            CartId = cartId;
            ProductVariantId = productVariantId;
            Quantity = quantity;
        }

        internal void IncreaseQuantity(int amount)
        {
            if (amount <= 0)
                throw new DomainException("Amount to increase must be greater than zero.");

            Quantity += amount;
        }

        internal void SetQuantity(int quantity)
        {
            if (quantity <= 0)
                throw new DomainException("Quantity must be greater than zero.");

            Quantity = quantity;
        }
    }
}
