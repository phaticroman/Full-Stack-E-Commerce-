using Cortex.Mediator.Queries;
using MaleFashion.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaleFashion.Application.Features.Products.Query
{
    public class GetHomeFeaturedProductsQuery : IQuery<IReadOnlyList<HomeProductDto>>
    {
        public int Take { get; set; } = 8;

        public GetHomeFeaturedProductsQuery(int take = 8)
        {
            Take = take;
        }
    }
}
