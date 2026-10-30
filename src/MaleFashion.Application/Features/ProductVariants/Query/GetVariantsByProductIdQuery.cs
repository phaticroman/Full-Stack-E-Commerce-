using Cortex.Mediator.Queries;
using MaleFashion.Domain.Entites;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaleFashion.Application.Features.ProductVariants.Query
{
    public class GetVariantsByProductIdQuery : IQuery<IList<ProductVariant>>
    {
        public Guid ProductId { get; }

        public GetVariantsByProductIdQuery(Guid productId)
        {
            ProductId = productId;
        }
    }
}
