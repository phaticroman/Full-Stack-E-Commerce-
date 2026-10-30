using Cortex.Mediator.Commands;
using MaleFashion.Application.Contracts;
using MaleFashion.Application.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;
using MaleFashion.Domain.Enums;

namespace MaleFashion.Application.Features.Orders.Command
{
    public class UpdateOrderStatusCommandHandler : ICommandHandler<UpdateOrderStatusCommand, bool>
    {
        private readonly IApplicationUnitOfwork _unitOfWork;

        public UpdateOrderStatusCommandHandler(IApplicationUnitOfwork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(UpdateOrderStatusCommand command, CancellationToken cancellationToken)
        {
            var order = await _unitOfWork.OrderRepository.GetByIdAsync(command.OrderId, cancellationToken);
            if (order == null)
            {
                throw new NotFoundException($"Order with ID {command.OrderId} was not found.");
            }

            if (order.Status == command.NewStatus)
            {
                return true;
            }
            switch (command.NewStatus)
            {
                case OrderStatus.Confirmed:
                    order.ConfirmPayment();
                    break;

                case OrderStatus.Shipped:
                    order.MarkShipped();
                    break;

                case OrderStatus.Delivered:
                    order.MarkDelivered();
                    break;

                case OrderStatus.Cancelled:
                    order.Cancel();
                    break;

                default:
                    throw new DomainException($"Cannot directly transition order status from '{order.Status}' to '{command.NewStatus}'.");
            }

            await _unitOfWork.SaveAsync(cancellationToken);
            return true;
        }
    }
}
