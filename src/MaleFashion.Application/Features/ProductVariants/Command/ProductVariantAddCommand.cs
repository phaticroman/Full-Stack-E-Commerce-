using Cortex.Mediator.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaleFashion.Application.Features.ProductVariants.Command
{
    public class ProductVariantAddCommand : ICommand<Guid>
    {
        public Guid ProductId { get; set; }
        public string SKU { get; set; } = null!;
        public string Size { get; set; } = null!;
        public string Color { get; set; } = null!;
        public int StockQuantity { get; set; }
    }
}
