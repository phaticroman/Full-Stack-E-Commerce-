using Cortex.Mediator.Queries;
using MaleFashion.Application.Contracts;
using MaleFashion.Domain.Entites;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaleFashion.Application.Features.Products.Query
{
    public class GetProductByIdWithDetailsQueryHandler
        : IQueryHandler<GetProductByIdWithDetailsQuery, Product?>
    {
        private readonly IApplicationUnitOfwork _unitOfWork;

        public GetProductByIdWithDetailsQueryHandler(IApplicationUnitOfwork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Product?> Handle(
            GetProductByIdWithDetailsQuery query,
            CancellationToken cancellationToken)
        {
            return await _unitOfWork.ProductRepository.GetByIdWithDetailsAsync(query.Id, cancellationToken);
        }
    }
}
