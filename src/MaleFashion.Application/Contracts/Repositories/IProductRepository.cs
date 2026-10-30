
using MaleFashion.Domain.Contracts;
using MaleFashion.Domain.Entites;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaleFashion.Application.Contracts.Repositories
{
    public interface IProductRepository : IRepository<Product, Guid>
    {

        Task<Product?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
        Task<Product?> GetByVariantIdWithDetailsAsync(Guid variantId, CancellationToken cancellationToken = default);
        Task<Product?> GetByImageIdWithDetailsAsync(Guid imageId, CancellationToken cancellationToken = default);

        Task<IList<ProductImage>> GetImagesByProductIdAsync(Guid productId, CancellationToken cancellationToken = default);
        Task<IList<ProductVariant>> GetVariantsByProductIdAsync(Guid productId, CancellationToken cancellationToken = default);

        Task<bool> IsDuplicateProductNameAsync(string name, Guid? id, CancellationToken cancellationToken = default);
        Task<bool> IsDuplicateSkuAsync(string sku, Guid? excludeVariantId, CancellationToken cancellationToken = default);
        Task<IList<Product>> GetRelatedProductsAsync(Guid categoryId, Guid excludeProductId, int take, CancellationToken cancellationToken = default);
        Task<Dictionary<Guid, int>> GetActiveProductCountsByCategoryAsync(CancellationToken cancellationToken = default);
        Task<(IList<string> Sizes, IList<string> Colors)> GetDistinctVariantsFilterOptionsAsync(CancellationToken cancellationToken = default);

        Task<(IList<Product> Items, int Total, int TotalDisplay)> GetPagedProductsAsync(
            string? searchText,
            Guid? categoryId,
            string? sortText,
            int pageIndex,
            int pageSize,
            CancellationToken cancellationToken = default);

        Task<(IList<Product> Items, int Total, int TotalDisplay)> GetStorefrontProductsAsync(
            Guid? categoryId,
            string? searchText,
            decimal? priceFrom,
            decimal? priceTo,
            string? size,
            string? color,
            string? sortBy,
            int pageIndex,
            int pageSize,
            CancellationToken cancellationToken = default);

        Task<IList<Product>> GetFeaturedProductsAsync(int take, CancellationToken cancellationToken = default);
    }
}
