using MaleFashion.Application.Contracts.Repositories;
using MaleFashion.Domain.Entites;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaleFashion.Infrastructure.Data.Respositories
{
    public class CategoryRepository : Repository<Category, Guid>, ICategoryRepository
    {
        private readonly ApplicationDbContext _context;

        public CategoryRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<bool> IsDuplicateCategorySlug(string slug, Guid? id, CancellationToken cancellationToken = default)
        {
            if (!id.HasValue)
                return (await GetCountAsync(x => x.Slug == slug, cancellationToken)) > 0;

            return (await GetCountAsync(x => x.Slug == slug && x.Id != id.Value, cancellationToken)) > 0;
        }

        public async Task<Category?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default)
        {
            return await _context.Categories
                .FirstOrDefaultAsync(c => c.Slug == slug, cancellationToken);
        }

        public async Task<IList<Category>> GetActiveCategoriesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Categories
                .Where(c => c.IsActive)
                .OrderBy(c => c.DisplayOrder)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<(IList<Category> Items, int Total, int TotalDisplay)> GetPagedCategoriesAsync(
            string? searchText,
            string? sortText,
            int pageIndex,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            return await GetDynamicAsync(
                filter: x => searchText == null || x.Name.Contains(searchText),
                orderBy: sortText ?? "DisplayOrder ASC",
                include: null,
                pageIndex: pageIndex,
                pageSize: pageSize,
                isTrackingOff: true,
                cancellationToken: cancellationToken);
        }
        
    }
}
