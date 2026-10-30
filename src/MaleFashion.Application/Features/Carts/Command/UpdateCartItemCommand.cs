using Cortex.Mediator.Commands;
using MaleFashion.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaleFashion.Application.Features.Carts.Command
{
    public class UpdateCartItemCommand : ICommand<CartDto>
    {
        public Guid CustomerId { get; set; }
        public Guid CartItemId { get; set; }
        public int Quantity { get; set; }
    }
}
