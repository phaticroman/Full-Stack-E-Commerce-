using Cortex.Mediator.Queries;
using MaleFashion.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaleFashion.Application.Features.Carts.Query
{
    public class GetCartByCustomerIdQuery : IQuery<CartDto>
    {
        public Guid CustomerId { get; set; }

        public GetCartByCustomerIdQuery(Guid customerId)
        {
            CustomerId = customerId;
        }
    }
}
