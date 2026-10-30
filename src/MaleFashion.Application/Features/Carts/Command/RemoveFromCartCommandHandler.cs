using Cortex.Mediator.Commands;
using MaleFashion.Application.Contracts;
using MaleFashion.Application.DTOs;
using MaleFashion.Application.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaleFashion.Application.Features.Carts.Command
{
    public class RemoveFromCartCommandHandler : ICommandHandler<RemoveFromCartCommand, CartDto>
    {
        private readonly IApplicationUnitOfwork _unitOfWork;

        public RemoveFromCartCommandHandler(IApplicationUnitOfwork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<CartDto> Handle(RemoveFromCartCommand command, CancellationToken cancellationToken)
        {
            var cart = await _unitOfWork.CartRepository.GetByCustomerIdWithItemsAsync(command.CustomerId, cancellationToken);
            if (cart == null)
                throw new NotFoundException("Cart not found.");

            cart.RemoveItem(command.CartItemId);

            await _unitOfWork.SaveAsync(cancellationToken);

            var updatedCart = await _unitOfWork.CartRepository.GetByCustomerIdWithItemsAsync(command.CustomerId, cancellationToken);
            return updatedCart!.ToDto();
        }
    }
}
