using MaleFashion.Application.Contracts.Repositories;
using MaleFashion.Domain.Entites;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaleFashion.Infrastructure.Data.Respositories
{
    public class ReviewRepository : Repository<Review, Guid>, IReviewRepository
    {
        private readonly ApplicationDbContext _context;
        public ReviewRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<bool> HasCustomerReviewedProductAsync(
            Guid productId,
            Guid customerId,
            CancellationToken cancellationToken = default)
        {
            return await _context.Reviews.AnyAsync(
                r => r.ProductId == productId && r.CustomerId == customerId,
                cancellationToken);
        }

        public async Task<IReadOnlyList<Review>> GetByProductIdAsync(
            Guid productId,
            CancellationToken cancellationToken = default)
        {
            return await _context.Reviews
                .Where(r => r.ProductId == productId)
                .OrderByDescending(r => r.CreatedAt)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<(double AverageRating, int TotalReviews)> GetProductRatingSummaryAsync(
            Guid productId,
            CancellationToken cancellationToken = default)
        {
            var stats = await _context.Reviews
                .Where(r => r.ProductId == productId)
                .GroupBy(r => r.ProductId)
                .Select(g => new
                {
                    Average = g.Average(r => (double)r.Rating),
                    Count = g.Count()
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (stats == null)
            {
                return (0.0, 0);
            }

            return (Math.Round(stats.Average, 1), stats.Count);
        }
    }
}
