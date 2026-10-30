
using MaleFashion.Application.Features.Orders.Query;
using MaleFashion.Domain.Contracts;
using MaleFashion.Domain.Entites;
using MaleFashion.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaleFashion.Application.Contracts.Repositories
{
    public interface IOrderRepository : IRepository<Order, Guid>
    {
        Task<Order?> GetByIdWithItemsAsync(Guid orderId, CancellationToken cancellationToken = default);

        Task<IList<Order>> GetOrdersByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default);

        Task<bool> HasOrdersAsync(Guid customerId, CancellationToken cancellationToken = default);

        Task<(IList<Order> Orders, int TotalCount)> GetCustomerOrdersPaginatedAsync(
            Guid customerId,
            int pageIndex,
            int pageSize,
            CancellationToken cancellationToken = default);
        Task<decimal> GetTotalSalesAsync(CancellationToken cancellationToken = default);
        Task<IList<Order>> GetRecentOrdersAsync(int count, CancellationToken cancellationToken = default);
        Task<(IList<Order> Orders, int TotalCount)> GetAdminOrdersPaginatedAsync(
    string? searchTerm,
    OrderStatus? status,
    int pageIndex,
    int pageSize,
    CancellationToken cancellationToken = default);
    }
}
