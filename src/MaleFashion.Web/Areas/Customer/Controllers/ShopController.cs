using Cortex.Mediator;
using MaleFashion.Application.Features.Categories.Query;
using MaleFashion.Application.Features.Products.Query;
using MaleFashion.Application.Features.Reviews.Query;
using MaleFashion.Web.Areas.Customer.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MaleFashion.Web.Areas.Customer.Controllers
{
    [Area("Customer")]

    public class ShopController : Controller
    {
        private readonly IMediator _mediator;

        public ShopController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> Index([FromQuery] ShopFilterModel filter, CancellationToken cancellationToken)
        {
            var productQuery = new GetStorefrontProductsQuery
            {
                CategoryId = filter.CategoryId,
                SearchText = filter.SearchText,
                PriceFrom = filter.PriceFrom,
                PriceTo = filter.PriceTo,
                Size = filter.Size,
                Color = filter.Color,
                SortBy = filter.SortBy,
                PageIndex = filter.PageIndex < 1 ? 1 : filter.PageIndex,
                PageSize = filter.PageSize < 1 ? 9 : filter.PageSize
            };

            var (items, total, filteredCount) = await _mediator.SendQueryAsync(productQuery, cancellationToken);
            var categories = await _mediator.SendQueryAsync(new GetActiveCategoriesQuery(), cancellationToken);
            var (sizes, colors) = await _mediator.SendQueryAsync(new GetProductFilterOptionsQuery(), cancellationToken);

            var model = new ShopIndexViewModel
            {
                Products = items,
                TotalItems = total,
                FilteredCount = filteredCount,
                Filter = filter,
                Categories = categories,
                AvailableSizes = sizes,
                AvailableColors = colors
            };

            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> Details(Guid id, CancellationToken cancellationToken)
        {
            var product = await _mediator.SendQueryAsync(new GetProductByIdWithDetailsQuery(id), cancellationToken);
            if (product == null || !product.IsActive)
            {
                return NotFound();
            }

            var related = await _mediator.SendQueryAsync(
                new GetRelatedProductsQuery(product.CategoryId, product.Id, take: 4),
                cancellationToken);

            var reviews = await _mediator.SendQueryAsync(new GetProductReviewsQuery(product.Id), cancellationToken);

            var currentUserIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Guid.TryParse(currentUserIdString, out var currentUserId);

            var reviewItems = reviews.Select(r => new ProductReviewItemModel
            {
                Id = r.Id,
                CustomerId = r.CustomerId,
                CustomerName = r.CustomerName ?? "Customer",
                Rating = r.Rating,
                Comment = r.Comment,
                CreatedAt = r.CreatedAt,
                IsOwner = currentUserId != Guid.Empty && r.CustomerId == currentUserId
            }).ToList();

            var model = new ProductDetailsViewModel
            {
                Id = product.Id,
                Name = product.Name,
                Slug = product.Slug,
                Description = product.Description,
                Price = product.Price,
                DiscountPrice = product.DiscountPrice,
                Brand = product.Brand,
                ImageName = product.ImageName,
                CategoryName = product.Category?.Name ?? "Uncategorized",
                Images = product.Images.OrderBy(i => i.DisplayOrder).ToList(),
                Variants = product.Variants.ToList(),
                RelatedProducts = related,

                Reviews = reviewItems,
                ReviewCount = reviewItems.Count,
                AverageRating = reviewItems.Count > 0 ? Math.Round(reviewItems.Average(r => (double)r.Rating), 1) : 0,
                HasCustomerReviewed = currentUserId != Guid.Empty && reviewItems.Any(r => r.CustomerId == currentUserId)
            };

            return View(model);
        }
    }
}
