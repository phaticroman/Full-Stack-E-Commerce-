using Cortex.Mediator.Queries;
using MaleFashion.Application.Contracts;
using MaleFashion.Domain.Entites;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaleFashion.Application.Features.ProductImages.Query
{
    public class GetImagesByProductIdQueryHandler
        : IQueryHandler<GetImagesByProductIdQuery, IList<ProductImage>>
    {
        private readonly IApplicationUnitOfwork _unitOfWork;

        public GetImagesByProductIdQueryHandler(IApplicationUnitOfwork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IList<ProductImage>> Handle(
            GetImagesByProductIdQuery query,
            CancellationToken cancellationToken)
        {
            return await _unitOfWork.ProductRepository.GetImagesByProductIdAsync(query.ProductId, cancellationToken);
        }
    }
}
