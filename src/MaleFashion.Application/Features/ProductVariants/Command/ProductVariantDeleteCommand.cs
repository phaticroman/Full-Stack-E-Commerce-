using Cortex.Mediator.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaleFashion.Application.Features.ProductVariants.Command
{
    public class ProductVariantDeleteCommand : ICommand
    {
        public Guid Id { get; set; }

        public ProductVariantDeleteCommand(Guid id)
        {
            Id = id;
        }

        public ProductVariantDeleteCommand() { }
    }
}
