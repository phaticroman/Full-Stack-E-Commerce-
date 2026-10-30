using Cortex.Mediator.Queries;
using MaleFashion.Application.Contracts;
using MaleFashion.Domain.Entites;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaleFashion.Application.Features.Products.Query
{
    public class GetPagedProductsQueryHandler
        : IQueryHandler<GetPagedProductsQuery, (IList<Product> Items, int Total, int TotalDisplay)>
    {
        private readonly IApplicationUnitOfwork _unitOfWork;

        public GetPagedProductsQueryHandler(IApplicationUnitOfwork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<(IList<Product> Items, int Total, int TotalDisplay)> Handle(
            GetPagedProductsQuery query,
            CancellationToken cancellationToken)
        {
            return await _unitOfWork.ProductRepository.GetPagedProductsAsync(
                searchText: query.SearchText,
                categoryId: query.CategoryId,
                sortText: query.SortText,
                pageIndex: query.PageIndex,
                pageSize: query.PageSize,
                cancellationToken: cancellationToken);
        }
    }
}
