using Cortex.Mediator.Queries;
using MaleFashion.Domain.Entites;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaleFashion.Application.Features.ProductImages.Query
{
    public class GetProductImageByIdQuery : IQuery<ProductImage?>
    {
        public Guid ImageId { get; }

        public GetProductImageByIdQuery(Guid imageId)
        {
            ImageId = imageId;
        }
    }
}
