using Cortex.Mediator.Queries;
using MaleFashion.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaleFashion.Application.Features.Orders.Query
{
    public class GetOrderDetailsForAdminQuery : IQuery<AdminOrderDetailsDto?>
    {
        public Guid OrderId { get; set; }

        public GetOrderDetailsForAdminQuery(Guid orderId)
        {
            OrderId = orderId;
        }
    }
}
