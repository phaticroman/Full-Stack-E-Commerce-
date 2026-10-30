using Cortex.Mediator.Queries;
using MaleFashion.Domain.Entites;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaleFashion.Application.Features.Products.Query
{
    public class GetPagedProductsQuery : IQuery<(IList<Product> Items, int Total, int TotalDisplay)>
    {
        public string? SearchText { get; set; }
        public Guid? CategoryId { get; set; }
        public string? SortText { get; set; }
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
