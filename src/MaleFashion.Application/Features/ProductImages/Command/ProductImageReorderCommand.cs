using Cortex.Mediator.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaleFashion.Application.Features.ProductImages.Command
{
    public class ProductImageReorderCommand : ICommand
    {
        public Guid ImageId { get; set; }
        public int NewDisplayOrder { get; set; }
    }
}
