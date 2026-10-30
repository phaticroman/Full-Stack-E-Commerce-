using MaleFashion.Application.Contracts.Repositories;
using MaleFashion.Domain.Entites;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaleFashion.Infrastructure.Data.Respositories
{
    public class CartRepository : Repository<Cart, Guid>, ICartRepository
    {
        private readonly ApplicationDbContext _context;

        public CartRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Cart?> GetByCustomerIdWithItemsAsync(Guid customerId, CancellationToken cancellationToken = default)
        {
            return await _context.Carts
                .Include(c => c.Items)
                    .ThenInclude(i => i.ProductVariant)
                        .ThenInclude(v => v!.Product)
                .FirstOrDefaultAsync(c => c.CustomerId == customerId, cancellationToken);
        }

        public async Task<bool> HasCartAsync(Guid customerId, CancellationToken cancellationToken = default)
        {
            if (customerId == Guid.Empty)
                return false;

            return await _context.Carts.AnyAsync(c => c.CustomerId == customerId, cancellationToken);
        }

        public async Task<int> GetCartItemCountAsync(Guid customerId, CancellationToken cancellationToken = default)
        {
            if (customerId == Guid.Empty)
                return 0;

            return await _context.Carts
                .AsNoTracking()
                .Where(c => c.CustomerId == customerId)
                .SelectMany(c => c.Items)
                .SumAsync(i => (int?)i.Quantity, cancellationToken) ?? 0;
        }

        public async Task ClearCartAsync(Guid cartId, CancellationToken cancellationToken = default)
        {
            var cart = await _context.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.Id == cartId, cancellationToken);

            if (cart != null)
            {
                cart.Clear();
            }
        }
    }
}
