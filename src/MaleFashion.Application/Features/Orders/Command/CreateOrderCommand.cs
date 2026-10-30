using Cortex.Mediator.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaleFashion.Application.Features.Orders.Command
{
    public class CreateOrderCommand : ICommand<Guid>
    {
        public Guid CustomerId { get; set; }
        public string ShippingFullName { get; set; } = null!;
        public string ShippingPhone { get; set; } = null!;
        public string ShippingEmail { get; set; } = null!;
        public string ShippingCity { get; set; } = null!;
        public string ShippingPostalCode { get; set; } = null!;
    }
}
