using Cortex.Mediator.Queries;
using MaleFashion.Application.Contracts;
using MaleFashion.Domain.Entites;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaleFashion.Application.Features.ProductVariants.Query
{
    public class GetVariantsByProductIdQueryHandler
        : IQueryHandler<GetVariantsByProductIdQuery, IList<ProductVariant>>
    {
        private readonly IApplicationUnitOfwork _unitOfWork;

        public GetVariantsByProductIdQueryHandler(IApplicationUnitOfwork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IList<ProductVariant>> Handle(
            GetVariantsByProductIdQuery query,
            CancellationToken cancellationToken)
        {
            return await _unitOfWork.ProductRepository.GetVariantsByProductIdAsync(query.ProductId, cancellationToken);
        }
    }
}
