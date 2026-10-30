using Cortex.Mediator.Queries;
using MaleFashion.Application.Contracts;
using MaleFashion.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaleFashion.Application.Features.Dashboard.Query
{
    public class GetDashboardStatsQueryHandler : IQueryHandler<GetDashboardStatsQuery, DashboardStatsDto>
    {
        private readonly IApplicationUnitOfwork _unitOfWork;

        public GetDashboardStatsQueryHandler(IApplicationUnitOfwork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<DashboardStatsDto> Handle(GetDashboardStatsQuery query, CancellationToken cancellationToken)
        {
            var totalSales = await _unitOfWork.OrderRepository.GetTotalSalesAsync(cancellationToken);
            var totalOrders = await _unitOfWork.OrderRepository.GetCountAsync(null, cancellationToken);
            var totalProducts = await _unitOfWork.ProductRepository.GetCountAsync(p => p.IsActive, cancellationToken);
            var recentOrders = await _unitOfWork.OrderRepository.GetRecentOrdersAsync(5, cancellationToken);

            return new DashboardStatsDto
            {
                TotalSales = totalSales,
                TotalOrders = totalOrders,
                TotalProducts = totalProducts,
                RecentOrders = recentOrders.Select(o => new RecentOrderDto
                {
                    Id = o.Id,
                    ShippingFullName = o.ShippingFullName,
                    OrderDate = o.OrderDate,
                    Total = o.Total,
                    Status = o.Status
                }).ToList()
            };
        }
    }
}
