using Cortex.Mediator.Queries;
using MaleFashion.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaleFashion.Application.Features.Reviews.Query
{
    public class GetProductReviewsQuery : IQuery<IReadOnlyList<ReviewDto>>
    {
        public Guid ProductId { get; set; }

        public GetProductReviewsQuery(Guid productId)
        {
            ProductId = productId;
        }
    }
}
