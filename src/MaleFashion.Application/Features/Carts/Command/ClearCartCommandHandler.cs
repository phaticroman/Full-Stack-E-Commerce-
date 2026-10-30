using Cortex.Mediator.Commands;
using MaleFashion.Application.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaleFashion.Application.Features.Carts.Command
{
    public class ClearCartCommandHandler : ICommandHandler<ClearCartCommand>
    {
        private readonly IApplicationUnitOfwork _unitOfWork;

        public ClearCartCommandHandler(IApplicationUnitOfwork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(ClearCartCommand command, CancellationToken cancellationToken)
        {
            var cart = await _unitOfWork.CartRepository.GetByCustomerIdWithItemsAsync(command.CustomerId, cancellationToken);
            if (cart != null)
            {
                cart.Clear();
                await _unitOfWork.SaveAsync(cancellationToken);
            }
        }
    }
}
