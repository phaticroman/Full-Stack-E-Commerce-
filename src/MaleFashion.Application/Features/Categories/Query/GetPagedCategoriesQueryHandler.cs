using Cortex.Mediator.Queries;
using MaleFashion.Application.Contracts;
using MaleFashion.Domain.Entites;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaleFashion.Application.Features.Categories.Query
{
    public class GetPagedCategoriesQueryHandler
        : IQueryHandler<GetPagedCategoriesQuery, (IList<Category> Items, int Total, int TotalDisplay)>
    {
        private readonly IApplicationUnitOfwork _unitOfWork;

        public GetPagedCategoriesQueryHandler(IApplicationUnitOfwork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<(IList<Category> Items, int Total, int TotalDisplay)> Handle(
            GetPagedCategoriesQuery query,
            CancellationToken cancellationToken)
        {
            return await _unitOfWork.CategoryRepository.GetPagedCategoriesAsync(
                searchText: query.SearchText,
                sortText: query.SortText,
                pageIndex: query.PageIndex,
                pageSize: query.PageSize,
                cancellationToken: cancellationToken);
        }
    }
}
