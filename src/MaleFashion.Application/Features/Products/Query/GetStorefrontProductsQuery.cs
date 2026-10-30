using Cortex.Mediator.Queries;
using MaleFashion.Domain.Entites;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaleFashion.Application.Features.Products.Query
{
    public class GetStorefrontProductsQuery : IQuery<(IList<Product> Items, int Total, int TotalDisplay)>
    {
        public Guid? CategoryId { get; set; }
        public string? SearchText { get; set; }
        public decimal? PriceFrom { get; set; }
        public decimal? PriceTo { get; set; }
        public string? Size { get; set; }
        public string? Color { get; set; }
        public string? SortBy { get; set; }
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 9;
    }
}
