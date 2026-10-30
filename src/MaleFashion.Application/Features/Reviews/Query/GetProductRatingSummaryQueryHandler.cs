using Cortex.Mediator.Queries;
using MaleFashion.Application.Contracts;
using MaleFashion.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaleFashion.Application.Features.Reviews.Query
{
    public class GetProductRatingSummaryQueryHandler : IQueryHandler<GetProductRatingSummaryQuery, ProductRatingSummaryDto>
    {
        private readonly IApplicationUnitOfwork _unitOfWork;

        public GetProductRatingSummaryQueryHandler(IApplicationUnitOfwork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ProductRatingSummaryDto> Handle(GetProductRatingSummaryQuery query, CancellationToken cancellationToken = default)
        {
            var summary = await _unitOfWork.ReviewRepository.GetProductRatingSummaryAsync(query.ProductId, cancellationToken);

            return new ProductRatingSummaryDto
            {
                AverageRating = summary.AverageRating,
                TotalReviews = summary.TotalReviews
            };
        }
    }
}
