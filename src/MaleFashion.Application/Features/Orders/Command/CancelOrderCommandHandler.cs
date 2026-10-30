using Cortex.Mediator.Commands;
using MaleFashion.Application.Contracts;
using MaleFashion.Application.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaleFashion.Application.Features.Orders.Command
{
    public class CancelOrderCommandHandler : ICommandHandler<CancelOrderCommand>
    {
        private readonly IApplicationUnitOfwork _unitOfWork;

        public CancelOrderCommandHandler(IApplicationUnitOfwork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(CancelOrderCommand command, CancellationToken cancellationToken)
        {

            var order = await _unitOfWork.OrderRepository.GetByIdWithItemsAsync(command.OrderId, cancellationToken);
            if (order == null || order.CustomerId != command.CustomerId)
                throw new NotFoundException("Order not found or access denied.");


            order.Cancel();

            foreach (var item in order.Items)
            {
                if (item.ProductVariant != null)
                {
                    item.ProductVariant.Restock(item.Quantity);
                }
            }

            await _unitOfWork.SaveAsync(cancellationToken);
        }
    }
}
