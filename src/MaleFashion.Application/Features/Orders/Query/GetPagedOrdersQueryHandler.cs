using Cortex.Mediator.Queries;
using MaleFashion.Application.Contracts;
using MaleFashion.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaleFashion.Application.Features.Orders.Query
{
    public class GetPagedOrdersQueryHandler : IQueryHandler<GetPagedOrdersQuery, PagedOrdersResultDto>
    {
        private readonly IApplicationUnitOfwork _unitOfWork;

        public GetPagedOrdersQueryHandler(IApplicationUnitOfwork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PagedOrdersResultDto> Handle(GetPagedOrdersQuery query, CancellationToken cancellationToken)
        {
            var (orders, totalCount) = await _unitOfWork.OrderRepository.GetAdminOrdersPaginatedAsync(
                query.SearchTerm,
                query.Status,
                query.PageIndex,
                query.PageSize,
                cancellationToken);

            var items = orders.Select(o => new AdminOrderListItemDto
            {
                Id = o.Id,
                CustomerName = o.ShippingFullName,
                CustomerEmail = o.ShippingEmail,
                Total = o.Total,
                Status = o.Status,
                OrderDate = o.OrderDate,
                ItemCount = o.Items?.Count ?? 0
            }).ToList();

            return new PagedOrdersResultDto
            {
                Items = items,
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
    }
}
