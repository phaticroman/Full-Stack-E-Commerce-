using Cortex.Mediator.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaleFashion.Application.Features.Carts.Command
{
    public class ClearCartCommand : ICommand
    {
        public Guid CustomerId { get; set; }

        public ClearCartCommand(Guid customerId)
        {
            CustomerId = customerId;
        }
    }
}
