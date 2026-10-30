using Cortex.Mediator.Queries;
using MaleFashion.Application.Contracts;
using MaleFashion.Domain.Entites;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaleFashion.Application.Features.ProductImages.Query
{
    public class GetProductImageByIdQueryHandler
        : IQueryHandler<GetProductImageByIdQuery, ProductImage?>
    {
        private readonly IApplicationUnitOfwork _unitOfWork;

        public GetProductImageByIdQueryHandler(IApplicationUnitOfwork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ProductImage?> Handle(
            GetProductImageByIdQuery query,
            CancellationToken cancellationToken)
        {
            var product = await _unitOfWork.ProductRepository.GetByImageIdWithDetailsAsync(query.ImageId, cancellationToken);
            return product?.Images.FirstOrDefault(i => i.Id == query.ImageId);
        }
    }
}
