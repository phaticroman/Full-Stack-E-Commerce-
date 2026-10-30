using Cortex.Mediator.Queries;
using MaleFashion.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaleFashion.Application.Features.Orders.Query
{
    public class GetCustomerOrdersQuery : IQuery<IList<OrderDto>>
    {
        public Guid CustomerId { get; set; }

        public GetCustomerOrdersQuery(Guid customerId)
        {
            CustomerId = customerId;
        }
    }
}
