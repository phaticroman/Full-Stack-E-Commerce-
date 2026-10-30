using MaleFashion.Domain.Contracts;
using MaleFashion.Domain.Entites;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaleFashion.Application.Contracts.Repositories
{
    public interface ICartRepository : IRepository<Cart, Guid>
    {

        Task<Cart?> GetByCustomerIdWithItemsAsync(Guid customerId, CancellationToken cancellationToken = default);
        Task<bool> HasCartAsync(Guid customerId, CancellationToken cancellationToken = default);
        Task<int> GetCartItemCountAsync(Guid customerId, CancellationToken cancellationToken = default);
        Task ClearCartAsync(Guid cartId, CancellationToken cancellationToken = default);
    }
}
