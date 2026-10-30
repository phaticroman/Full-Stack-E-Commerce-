using Cortex.Mediator.Queries;
using MaleFashion.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaleFashion.Application.Features.Reviews.Query
{
    public class GetProductRatingSummaryQuery : IQuery<ProductRatingSummaryDto>
    {
        public Guid ProductId { get; set; }

        public GetProductRatingSummaryQuery(Guid productId)
        {
            ProductId = productId;
        }
    }
}
