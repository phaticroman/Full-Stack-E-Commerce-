using Cortex.Mediator.Commands;
using MaleFashion.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaleFashion.Application.Features.Orders.Command
{
    public class UpdateOrderStatusCommand : ICommand<bool>
    {
        public Guid OrderId { get; set; }
        public OrderStatus NewStatus { get; set; }

        public UpdateOrderStatusCommand(Guid orderId, OrderStatus newStatus)
        {
            OrderId = orderId;
            NewStatus = newStatus;
        }
    }
}
