using Cortex.Mediator.Queries;
using MaleFashion.Domain.Entites;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaleFashion.Application.Features.Products.Query
{
    public class GetRelatedProductsQuery : IQuery<IList<Product>>
    {
        public Guid CategoryId { get; }
        public Guid ExcludeProductId { get; }
        public int Take { get; }

        public GetRelatedProductsQuery(Guid categoryId, Guid excludeProductId, int take = 4)
        {
            CategoryId = categoryId;
            ExcludeProductId = excludeProductId;
            Take = take;
        }
    }
}
