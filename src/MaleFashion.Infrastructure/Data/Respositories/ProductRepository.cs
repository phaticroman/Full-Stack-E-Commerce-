using MaleFashion.Application.Contracts.Repositories;
using MaleFashion.Domain.Entites;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaleFashion.Infrastructure.Data.Respositories
{
    public class ProductRepository : Repository<Product, Guid>, IProductRepository
    {
        private readonly ApplicationDbContext _context;

        public ProductRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Product?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Images)
                .Include(p => p.Variants)
                .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        }

        public async Task<Product?> GetByVariantIdWithDetailsAsync(Guid variantId, CancellationToken cancellationToken = default)
        {
            return await _context.Products
                .Include(p => p.Variants)
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.Variants.Any(v => v.Id == variantId), cancellationToken);
        }

        public async Task<Product?> GetByImageIdWithDetailsAsync(Guid imageId, CancellationToken cancellationToken = default)
        {
            return await _context.Products
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.Images.Any(i => i.Id == imageId), cancellationToken);
        }

        public async Task<IList<ProductImage>> GetImagesByProductIdAsync(Guid productId, CancellationToken cancellationToken = default)
        {
            return await _context.ProductImages
                .Where(i => i.ProductId == productId)
                .OrderBy(i => i.DisplayOrder)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<IList<ProductVariant>> GetVariantsByProductIdAsync(Guid productId, CancellationToken cancellationToken = default)
        {
            return await _context.ProductVariants
                .Where(v => v.ProductId == productId)
                .OrderBy(v => v.Size)
                .ThenBy(v => v.Color)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> IsDuplicateProductNameAsync(string name, Guid? id, CancellationToken cancellationToken = default)
        {
            if (!id.HasValue)
                return (await GetCountAsync(x => x.Name == name, cancellationToken)) > 0;

            return (await GetCountAsync(x => x.Name == name && x.Id != id.Value, cancellationToken)) > 0;
        }

        public async Task<bool> IsDuplicateSkuAsync(string sku, Guid? excludeVariantId, CancellationToken cancellationToken = default)
        {
            var query = _context.ProductVariants.AsNoTracking();

            if (excludeVariantId.HasValue)
            {
                query = query.Where(v => v.Id != excludeVariantId.Value);
            }

            return await query.AnyAsync(v => v.SKU == sku, cancellationToken);
        }

        public async Task<IList<Product>> GetRelatedProductsAsync(Guid categoryId, Guid excludeProductId, int take, CancellationToken cancellationToken = default)
        {
            return await _context.Products
                .Include(p => p.Images)
                .Where(p => p.CategoryId == categoryId && p.Id != excludeProductId && p.IsActive)
                .OrderByDescending(p => p.CreatedAt)
                .Take(take)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<Dictionary<Guid, int>> GetActiveProductCountsByCategoryAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Products
                .Where(p => p.IsActive)
                .GroupBy(p => p.CategoryId)
                .Select(g => new { CategoryId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.CategoryId, x => x.Count, cancellationToken);
        }

        public async Task<(IList<string> Sizes, IList<string> Colors)> GetDistinctVariantsFilterOptionsAsync(CancellationToken cancellationToken = default)
        {
            var sizes = await _context.ProductVariants
                .Select(v => v.Size)
                .Distinct()
                .OrderBy(s => s)
                .ToListAsync(cancellationToken);

            var colors = await _context.ProductVariants
                .Select(v => v.Color)
                .Distinct()
                .OrderBy(c => c)
                .ToListAsync(cancellationToken);

            return (sizes, colors);
        }

        public async Task<(IList<Product> Items, int Total, int TotalDisplay)> GetPagedProductsAsync(
            string? searchText,
            Guid? categoryId,
            string? sortText,
            int pageIndex,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            return await GetDynamicAsync(
                filter: x => (string.IsNullOrWhiteSpace(searchText) || x.Name.Contains(searchText)) &&
                             (!categoryId.HasValue || x.CategoryId == categoryId.Value),
                orderBy: sortText ?? "CreatedAt DESC",
                include: q => (Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<Product, object>)q.Include(p => p.Category),
                pageIndex: pageIndex,
                pageSize: pageSize,
                isTrackingOff: true,
                cancellationToken: cancellationToken);
        }

        public async Task<(IList<Product> Items, int Total, int TotalDisplay)> GetStorefrontProductsAsync(
            Guid? categoryId,
            string? searchText,
            decimal? priceFrom,
            decimal? priceTo,
            string? size,
            string? color,
            string? sortBy,
            int pageIndex,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            var totalActive = await _context.Products.CountAsync(p => p.IsActive, cancellationToken);

            var baseQuery = _context.Products
                .Include(p => p.Images)
                .Where(p => p.IsActive);

            if (categoryId.HasValue)
                baseQuery = baseQuery.Where(p => p.CategoryId == categoryId.Value);

            if (!string.IsNullOrWhiteSpace(searchText))
                baseQuery = baseQuery.Where(p => p.Name.Contains(searchText));

            if (priceFrom.HasValue)
                baseQuery = baseQuery.Where(p => (p.DiscountPrice ?? p.Price) >= priceFrom.Value);

            if (priceTo.HasValue)
                baseQuery = baseQuery.Where(p => (p.DiscountPrice ?? p.Price) <= priceTo.Value);

            if (!string.IsNullOrWhiteSpace(size))
                baseQuery = baseQuery.Where(p => p.Variants.Any(v => v.Size == size));

            if (!string.IsNullOrWhiteSpace(color))
                baseQuery = baseQuery.Where(p => p.Variants.Any(v => v.Color == color));

            var filteredCount = await baseQuery.CountAsync(cancellationToken);

            baseQuery = sortBy switch
            {
                "price_asc" => baseQuery.OrderBy(p => p.DiscountPrice ?? p.Price),
                "price_desc" => baseQuery.OrderByDescending(p => p.DiscountPrice ?? p.Price),
                "name" => baseQuery.OrderBy(p => p.Name),
                _ => baseQuery.OrderByDescending(p => p.CreatedAt)
            };

            var items = await baseQuery
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            return (items, totalActive, filteredCount);
        }
        public async Task<IList<Product>> GetFeaturedProductsAsync(int take, CancellationToken cancellationToken = default)
        {
            return await _context.Products
                .Where(p => p.IsActive)
                .OrderByDescending(p => p.CreatedAt)
                .Take(take)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }
    }
}
