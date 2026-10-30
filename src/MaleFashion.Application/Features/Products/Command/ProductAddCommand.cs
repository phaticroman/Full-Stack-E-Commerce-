using Cortex.Mediator.Commands;
using MaleFashion.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaleFashion.Application.Features.Products.Command
{
    public class ProductAddCommand : ICommand<Guid>
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public decimal? DiscountPrice { get; set; }
        public string Brand { get; set; } = null!;
        public string ImageName { get; set; } = null!;
        public Guid CategoryId { get; set; }
        public bool IsActive { get; set; } = true;

        public IList<ProductVariantDto> Variants { get; set; } = new List<ProductVariantDto>();
        public IList<string> GalleryImages { get; set; } = new List<string>();
    }
}
