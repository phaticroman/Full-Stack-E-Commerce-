using Cortex.Mediator.Commands;
using MaleFashion.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaleFashion.Application.Features.Carts.Command
{
    public class AddToCartCommand : ICommand<CartDto>
    {
        public Guid CustomerId { get; set; }
        public Guid ProductVariantId { get; set; }
        public int Quantity { get; set; } = 1;
    }
}
