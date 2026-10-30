using Cortex.Mediator.Queries;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaleFashion.Application.Features.Carts.Query
{
    public class GetCartItemCountQuery : IQuery<int>
    {
        public Guid CustomerId { get; set; }

        public GetCartItemCountQuery(Guid customerId)
        {
            CustomerId = customerId;
        }
    }
}
