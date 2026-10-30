using Cortex.Mediator.Queries;
using MaleFashion.Application.Contracts;
using MaleFashion.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaleFashion.Application.Features.Reviews.Query
{
    public class GetProductReviewsQueryHandler : IQueryHandler<GetProductReviewsQuery, IReadOnlyList<ReviewDto>>
    {
        private readonly IApplicationUnitOfwork _unitOfWork;

        public GetProductReviewsQueryHandler(IApplicationUnitOfwork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IReadOnlyList<ReviewDto>> Handle(GetProductReviewsQuery query, CancellationToken cancellationToken = default)
        {
            var reviews = await _unitOfWork.ReviewRepository.GetByProductIdAsync(query.ProductId, cancellationToken);

            return reviews.Select(r => new ReviewDto
            {
                Id = r.Id,
                ProductId = r.ProductId,
                CustomerId = r.CustomerId,
                Rating = r.Rating,
                Comment = r.Comment,
                CreatedAt = r.CreatedAt
            }).ToList();
        }
    }
}
