using Cortex.Mediator.Queries;
using MaleFashion.Application.Contracts;
using MaleFashion.Domain.Entites;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaleFashion.Application.Features.Categories.Query
{
    public class GetCategoryByIdQueryHandler : IQueryHandler<GetCategoryByIdQuery, Category?>
    {
        private readonly IApplicationUnitOfwork _unitOfWork;

        public GetCategoryByIdQueryHandler(IApplicationUnitOfwork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Category?> Handle(GetCategoryByIdQuery query, CancellationToken cancellationToken)
        {
            return await _unitOfWork.CategoryRepository.GetByIdAsync(query.Id, cancellationToken);
        }
    }
}
