using MaleFashion.Application.Contracts.Repositories;
using MaleFashion.Domain.Entites;
using MaleFashion.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaleFashion.Infrastructure.Data.Respositories
{
    public class OrderRepository : Repository<Order, Guid>, IOrderRepository
    {
        private readonly ApplicationDbContext _context;

        public OrderRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Order?> GetByIdWithItemsAsync(Guid orderId, CancellationToken cancellationToken = default)
        {
            return await _context.Orders
        .Include(o => o.Items)
            .ThenInclude(i => i.ProductVariant)
        .FirstOrDefaultAsync(o => o.Id == orderId, cancellationToken);
        }

        public async Task<IList<Order>> GetOrdersByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default)
        {
            return await _context.Orders
                .Include(o => o.Items)
                .Where(o => o.CustomerId == customerId)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> HasOrdersAsync(Guid customerId, CancellationToken cancellationToken = default)
        {
            if (customerId == Guid.Empty)
                return false;

            return await _context.Orders.AnyAsync(o => o.CustomerId == customerId, cancellationToken);
        }

        public async Task<(IList<Order> Orders, int TotalCount)> GetCustomerOrdersPaginatedAsync(
            Guid customerId,
            int pageIndex,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            var query = _context.Orders
                .Include(o => o.Items)
                .Where(o => o.CustomerId == customerId);

            var totalCount = await query.CountAsync(cancellationToken);

            var orders = await query
                .OrderByDescending(o => o.OrderDate)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return (orders, totalCount);
        }
        public async Task<decimal> GetTotalSalesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Orders
                .Where(o => o.Status != OrderStatus.Cancelled)
                .SumAsync(o => (decimal?)o.Total, cancellationToken) ?? 0m;
        }
        public async Task<IList<Order>> GetRecentOrdersAsync(int count, CancellationToken cancellationToken = default)
        {
            return await _context.Orders
                .Include(o => o.Items)
                .OrderByDescending(o => o.OrderDate)
                .Take(count)
                .ToListAsync(cancellationToken);
        }
        public async Task<(IList<Order> Orders, int TotalCount)> GetAdminOrdersPaginatedAsync(
    string? searchTerm,
    OrderStatus? status,
    int pageIndex,
    int pageSize,
    CancellationToken cancellationToken = default)
        {
            var query = _context.Orders
                .Include(o => o.Items)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var search = searchTerm.Trim().ToLower();
                query = query.Where(o =>
                    o.ShippingFullName.ToLower().Contains(search) ||
                    o.ShippingEmail.ToLower().Contains(search) ||
                    o.Id.ToString().ToLower().Contains(search));
            }

            if (status.HasValue)
            {
                query = query.Where(o => o.Status == status.Value);
            }

            var totalCount = await query.CountAsync(cancellationToken);

            var orders = await query
                .OrderByDescending(o => o.OrderDate)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return (orders, totalCount);
        }
    }
}
