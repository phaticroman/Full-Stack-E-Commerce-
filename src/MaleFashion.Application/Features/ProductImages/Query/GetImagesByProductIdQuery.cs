using Cortex.Mediator.Queries;
using MaleFashion.Domain.Entites;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaleFashion.Application.Features.ProductImages.Query
{
    public class GetImagesByProductIdQuery : IQuery<IList<ProductImage>>
    {
        public Guid ProductId { get; }

        public GetImagesByProductIdQuery(Guid productId)
        {
            ProductId = productId;
        }
    }
}
