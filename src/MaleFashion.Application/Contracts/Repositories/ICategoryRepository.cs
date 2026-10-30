
using MaleFashion.Domain.Contracts;
using MaleFashion.Domain.Entites;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaleFashion.Application.Contracts.Repositories
{
    public interface ICategoryRepository : IRepository<Category,Guid>
    {
        Task<bool> IsDuplicateCategorySlug(string slug, Guid? id, CancellationToken cancellationToken = default);
        Task<Category?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default);
        Task<IList<Category>> GetActiveCategoriesAsync(CancellationToken cancellationToken = default);
        Task<(IList<Category> Items, int Total, int TotalDisplay)> GetPagedCategoriesAsync(
            string? searchText,
            string? sortText,
            int pageIndex,
            int pageSize,
            CancellationToken cancellationToken = default);
    }
}
