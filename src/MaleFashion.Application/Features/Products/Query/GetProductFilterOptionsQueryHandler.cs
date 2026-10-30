using Cortex.Mediator.Queries;
using MaleFashion.Application.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaleFashion.Application.Features.Products.Query
{
    public class GetProductFilterOptionsQueryHandler
        : IQueryHandler<GetProductFilterOptionsQuery, (IList<string> Sizes, IList<string> Colors)>
    {
        private readonly IApplicationUnitOfwork _unitOfWork;

        public GetProductFilterOptionsQueryHandler(IApplicationUnitOfwork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<(IList<string> Sizes, IList<string> Colors)> Handle(
            GetProductFilterOptionsQuery query,
            CancellationToken cancellationToken)
        {
            return await _unitOfWork.ProductRepository.GetDistinctVariantsFilterOptionsAsync(cancellationToken);
        }
    }
}
