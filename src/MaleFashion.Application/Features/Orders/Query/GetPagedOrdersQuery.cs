using Cortex.Mediator.Queries;
using MaleFashion.Application.DTOs;
using MaleFashion.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaleFashion.Application.Features.Orders.Query
{
    public class GetPagedOrdersQuery : IQuery<PagedOrdersResultDto>
    {
        public string? SearchTerm { get; set; }
        public OrderStatus? Status { get; set; }
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
