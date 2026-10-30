using Cortex.Mediator.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaleFashion.Application.Features.Orders.Command
{
    public class CancelOrderCommand : ICommand
    {
        public Guid OrderId { get; set; }
        public Guid CustomerId { get; set; }

        public CancelOrderCommand(Guid orderId, Guid customerId)
        {
            OrderId = orderId;
            CustomerId = customerId;
        }
    }
}
