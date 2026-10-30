using Cortex.Mediator.Queries;
using MaleFashion.Application.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaleFashion.Application.Features.Carts.Query
{
    public class GetCartItemCountQueryHandler : IQueryHandler<GetCartItemCountQuery, int>
    {
        private readonly IApplicationUnitOfwork _unitOfWork;

        public GetCartItemCountQueryHandler(IApplicationUnitOfwork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<int> Handle(GetCartItemCountQuery query, CancellationToken cancellationToken)
        {
            return await _unitOfWork.CartRepository.GetCartItemCountAsync(query.CustomerId, cancellationToken);
        }
    }
}
