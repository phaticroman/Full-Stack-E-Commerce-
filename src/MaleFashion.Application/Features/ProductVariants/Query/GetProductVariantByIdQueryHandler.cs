using Cortex.Mediator.Queries;
using MaleFashion.Application.Contracts;
using MaleFashion.Domain.Entites;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaleFashion.Application.Features.ProductVariants.Query
{
    public class GetProductVariantByIdQueryHandler
        : IQueryHandler<GetProductVariantByIdQuery, ProductVariant?>
    {
        private readonly IApplicationUnitOfwork _unitOfWork;

        public GetProductVariantByIdQueryHandler(IApplicationUnitOfwork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ProductVariant?> Handle(
            GetProductVariantByIdQuery query,
            CancellationToken cancellationToken)
        {
            var product = await _unitOfWork.ProductRepository.GetByVariantIdWithDetailsAsync(query.VariantId, cancellationToken);
            return product?.Variants.FirstOrDefault(v => v.Id == query.VariantId);
        }
    }
}
