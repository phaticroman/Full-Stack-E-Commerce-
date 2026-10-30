using Cortex.Mediator.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaleFashion.Application.Features.Products.Command
{
    public class ProductUpdateCommand : ICommand
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public decimal? DiscountPrice { get; set; }
        public string Brand { get; set; } = null!;
        public Guid CategoryId { get; set; }
        public string? ImageName { get; set; }
        public bool IsActive { get; set; }
    }
}
