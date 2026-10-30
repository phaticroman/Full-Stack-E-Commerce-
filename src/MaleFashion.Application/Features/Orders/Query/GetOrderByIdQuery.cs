using Cortex.Mediator.Queries;
using MaleFashion.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaleFashion.Application.Features.Orders.Query
{
    public class GetOrderByIdQuery : IQuery<OrderDto>
    {
        public Guid OrderId { get; set; }
        public Guid CustomerId { get; set; }

        public GetOrderByIdQuery(Guid orderId, Guid customerId)
        {
            OrderId = orderId;
            CustomerId = customerId;
        }
    }
}
