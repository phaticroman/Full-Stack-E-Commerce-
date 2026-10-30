using Cortex.Mediator.Queries;
using MaleFashion.Application.Contracts;
using MaleFashion.Domain.Entites;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaleFashion.Application.Features.Products.Query
{
    public class GetRelatedProductsQueryHandler : IQueryHandler<GetRelatedProductsQuery, IList<Product>>
    {
        private readonly IApplicationUnitOfwork _unitOfWork;

        public GetRelatedProductsQueryHandler(IApplicationUnitOfwork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IList<Product>> Handle(
            GetRelatedProductsQuery query,
            CancellationToken cancellationToken)
        {
            return await _unitOfWork.ProductRepository.GetRelatedProductsAsync(
                query.CategoryId,
                query.ExcludeProductId,
                query.Take,
                cancellationToken);
        }
    }
}
