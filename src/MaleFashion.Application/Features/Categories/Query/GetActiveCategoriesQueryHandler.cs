using Cortex.Mediator.Queries;
using MaleFashion.Application.Contracts;
using MaleFashion.Domain.Entites;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaleFashion.Application.Features.Categories.Query
{
    public class GetActiveCategoriesQueryHandler : IQueryHandler<GetActiveCategoriesQuery, IList<Category>>
    {
        private readonly IApplicationUnitOfwork _unitOfWork;

        public GetActiveCategoriesQueryHandler(IApplicationUnitOfwork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IList<Category>> Handle(GetActiveCategoriesQuery query, CancellationToken cancellationToken)
        {
            return await _unitOfWork.CategoryRepository.GetActiveCategoriesAsync(cancellationToken);
        }
    }
}
