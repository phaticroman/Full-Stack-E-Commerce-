using Cortex.Mediator.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaleFashion.Application.Features.ProductImages.Command
{
    public class ProductImageDeleteCommand : ICommand<string>
    {
        public Guid ImageId { get; set; }

        public ProductImageDeleteCommand(Guid imageId)
        {
            ImageId = imageId;
        }

        public ProductImageDeleteCommand() { }
    }
}
