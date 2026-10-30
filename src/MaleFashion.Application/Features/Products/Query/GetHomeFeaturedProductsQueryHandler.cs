using Cortex.Mediator.Queries;
using MaleFashion.Application.Contracts;
using MaleFashion.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaleFashion.Application.Features.Products.Query
{
    public class GetHomeFeaturedProductsQueryHandler : IQueryHandler<GetHomeFeaturedProductsQuery, IReadOnlyList<HomeProductDto>>
    {
        private readonly IApplicationUnitOfwork _unitOfWork;

        public GetHomeFeaturedProductsQueryHandler(IApplicationUnitOfwork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IReadOnlyList<HomeProductDto>> Handle(GetHomeFeaturedProductsQuery query, CancellationToken cancellationToken = default)
        {
            // GetAsync এর বদলে GetFeaturedProductsAsync ব্যবহার
            var products = await _unitOfWork.ProductRepository.GetFeaturedProductsAsync(query.Take, cancellationToken);

            var result = new List<HomeProductDto>();

            foreach (var p in products)
            {
                var summary = await _unitOfWork.ReviewRepository.GetProductRatingSummaryAsync(p.Id, cancellationToken);

                result.Add(new HomeProductDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Slug = p.Slug,
                    Price = p.Price,
                    DiscountPrice = p.DiscountPrice,
                    ImageName = p.ImageName,
                    CreatedAt = p.CreatedAt,
                    AverageRating = summary.TotalReviews > 0 ? summary.AverageRating : 5.0,
                    ReviewCount = summary.TotalReviews
                });
            }

            return result;
        }
    }
}
