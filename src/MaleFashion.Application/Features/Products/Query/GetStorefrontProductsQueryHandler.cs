using Cortex.Mediator.Queries;
using MaleFashion.Application.Contracts;
using MaleFashion.Domain.Entites;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaleFashion.Application.Features.Products.Query
{
    public class GetStorefrontProductsQueryHandler
        : IQueryHandler<GetStorefrontProductsQuery, (IList<Product> Items, int Total, int TotalDisplay)>
    {
        private readonly IApplicationUnitOfwork _unitOfWork;

        public GetStorefrontProductsQueryHandler(IApplicationUnitOfwork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<(IList<Product> Items, int Total, int TotalDisplay)> Handle(
            GetStorefrontProductsQuery query,
            CancellationToken cancellationToken)
        {
            return await _unitOfWork.ProductRepository.GetStorefrontProductsAsync(
                query.CategoryId,
                query.SearchText,
                query.PriceFrom,
                query.PriceTo,
                query.Size,
                query.Color,
                query.SortBy,
                query.PageIndex,
                query.PageSize,
                cancellationToken);
        }
    }
}
