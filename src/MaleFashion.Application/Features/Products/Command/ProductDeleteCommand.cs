using Cortex.Mediator.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaleFashion.Application.Features.Products.Command
{
    public class ProductDeleteCommand : ICommand
    {
        public Guid Id { get; set; }

        public ProductDeleteCommand(Guid id)
        {
            Id = id;
        }

        public ProductDeleteCommand() { }
    }
}
