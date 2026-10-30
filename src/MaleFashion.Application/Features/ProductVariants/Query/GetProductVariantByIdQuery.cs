using Cortex.Mediator.Queries;
using MaleFashion.Domain.Entites;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaleFashion.Application.Features.ProductVariants.Query
{
    public class GetProductVariantByIdQuery : IQuery<ProductVariant?>
    {
        public Guid VariantId { get; }

        public GetProductVariantByIdQuery(Guid variantId)
        {
            VariantId = variantId;
        }
    }
}
