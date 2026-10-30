using MaleFashion.Domain.Contracts;
using MaleFashion.Domain.Entites;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaleFashion.Application.Contracts.Repositories
{
    public interface IReviewRepository : IRepository<Review, Guid>
    {
        Task<bool> HasCustomerReviewedProductAsync(Guid productId, Guid customerId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Review>> GetByProductIdAsync(Guid productId, CancellationToken cancellationToken = default);
        Task<(double AverageRating, int TotalReviews)> GetProductRatingSummaryAsync(Guid productId, CancellationToken cancellationToken = default);
    }
}
