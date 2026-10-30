using Cortex.Mediator.Queries;
using MaleFashion.Domain.Entites;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaleFashion.Application.Features.Categories.Query
{
    public class GetPagedCategoriesQuery : IQuery<(IList<Category> Items, int Total, int TotalDisplay)>
    {
        public string? SearchText { get; set; }
        public string? SortText { get; set; }
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
