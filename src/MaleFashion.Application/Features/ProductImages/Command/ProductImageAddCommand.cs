using Cortex.Mediator.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaleFashion.Application.Features.ProductImages.Command
{
    public class ProductImageAddCommand : ICommand<Guid>
    {
        public Guid ProductId { get; set; }
        public string ImageName { get; set; } = null!;
    }
}
