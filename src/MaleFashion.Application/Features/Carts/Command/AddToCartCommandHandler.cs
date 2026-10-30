using Cortex.Mediator.Commands;
using MaleFashion.Application.Contracts;
using MaleFashion.Application.DTOs;
using MaleFashion.Application.Exceptions;
using MaleFashion.Domain.Entites;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaleFashion.Application.Features.Carts.Command
{
    public class AddToCartCommandHandler : ICommandHandler<AddToCartCommand, CartDto>
    {
        private readonly IApplicationUnitOfwork _unitOfWork;

        public AddToCartCommandHandler(IApplicationUnitOfwork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<CartDto> Handle(AddToCartCommand command, CancellationToken cancellationToken)
        {
            if (command.Quantity <= 0)
                throw new DomainException("Quantity must be at least 1.");

            var product = await _unitOfWork.ProductRepository.GetByVariantIdWithDetailsAsync(command.ProductVariantId, cancellationToken);
            if (product == null || !product.IsActive)
                throw new NotFoundException("Product variant not found or product is inactive.");

            var variant = product.Variants.FirstOrDefault(v => v.Id == command.ProductVariantId);
            if (variant == null)
                throw new NotFoundException("Product variant not found.");

            var cart = await _unitOfWork.CartRepository.GetByCustomerIdWithItemsAsync(command.CustomerId, cancellationToken);
            if (cart == null)
            {
                cart = Cart.StartFor(command.CustomerId, DateTime.UtcNow);
                await _unitOfWork.CartRepository.AddAsync(cart, cancellationToken);
            }

            var existingItem = cart.Items.FirstOrDefault(i => i.ProductVariantId == command.ProductVariantId);
            var projectedQuantity = (existingItem?.Quantity ?? 0) + command.Quantity;

            if (projectedQuantity > variant.StockQuantity)
                throw new DomainException($"Cannot add more items than available stock ({variant.StockQuantity} available).");

            cart.AddItem(command.ProductVariantId, command.Quantity);

            await _unitOfWork.SaveAsync(cancellationToken);

            var updatedCart = await _unitOfWork.CartRepository.GetByCustomerIdWithItemsAsync(command.CustomerId, cancellationToken);
            return updatedCart!.ToDto();
        }
    }
}
