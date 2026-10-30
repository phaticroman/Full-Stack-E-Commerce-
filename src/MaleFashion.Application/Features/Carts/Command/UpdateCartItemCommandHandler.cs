using Cortex.Mediator.Commands;
using MaleFashion.Application.Contracts;
using MaleFashion.Application.DTOs;
using MaleFashion.Application.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaleFashion.Application.Features.Carts.Command
{
    public class UpdateCartItemCommandHandler : ICommandHandler<UpdateCartItemCommand, CartDto>
    {
        private readonly IApplicationUnitOfwork _unitOfWork;

        public UpdateCartItemCommandHandler(IApplicationUnitOfwork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<CartDto> Handle(UpdateCartItemCommand command, CancellationToken cancellationToken)
        {
    
            var cart = await _unitOfWork.CartRepository.GetByCustomerIdWithItemsAsync(command.CustomerId, cancellationToken);
            if (cart == null)
                throw new NotFoundException("Cart not found.");

            var item = cart.Items.FirstOrDefault(i => i.Id == command.CartItemId);
            if (item == null)
                throw new NotFoundException("Cart item not found.");

            if (command.Quantity <= 0)
            {
                cart.RemoveItem(command.CartItemId);
            }
            else
            {
                var product = await _unitOfWork.ProductRepository.GetByVariantIdWithDetailsAsync(item.ProductVariantId, cancellationToken);
                var variant = product?.Variants.FirstOrDefault(v => v.Id == item.ProductVariantId);

                if (variant != null && command.Quantity > variant.StockQuantity)
                    throw new DomainException($"Cannot set quantity higher than available stock ({variant.StockQuantity}).");

                cart.UpdateItemQuantity(command.CartItemId, command.Quantity);
            }

            await _unitOfWork.SaveAsync(cancellationToken);

            var updatedCart = await _unitOfWork.CartRepository.GetByCustomerIdWithItemsAsync(command.CustomerId, cancellationToken);
            return updatedCart!.ToDto();
        }
    }
}
