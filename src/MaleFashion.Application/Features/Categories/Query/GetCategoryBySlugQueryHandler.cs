using Cortex.Mediator.Queries;
using MaleFashion.Application.Contracts;
using MaleFashion.Domain.Entites;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaleFashion.Application.Features.Categories.Query
{
    public class GetCategoryBySlugQueryHandler : IQueryHandler<GetCategoryBySlugQuery, Category?>
    {
        private readonly IApplicationUnitOfwork _unitOfWork;

        public GetCategoryBySlugQueryHandler(IApplicationUnitOfwork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Category?> Handle(GetCategoryBySlugQuery query, CancellationToken cancellationToken)
        {
            return await _unitOfWork.CategoryRepository.GetBySlugAsync(query.Slug, cancellationToken);
        }
    }
}
