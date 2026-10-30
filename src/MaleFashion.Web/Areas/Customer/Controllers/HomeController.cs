using Cortex.Mediator;
using MaleFashion.Application.DTOs;
using MaleFashion.Application.Features.Products.Query;
using MaleFashion.Web.Areas.Customer.Models;
using Microsoft.AspNetCore.Mvc;

namespace MaleFashion.Web.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly IMediator _mediator;

        public HomeController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {

            var productDtos = await _mediator.SendQueryAsync<GetHomeFeaturedProductsQuery, IReadOnlyList<HomeProductDto>>(
                new GetHomeFeaturedProductsQuery(take: 8),
                cancellationToken);

            var productItems = productDtos.Select(p => new HomeProductItemModel
            {
                Id = p.Id,
                Name = p.Name,
                Slug = p.Slug,
                Price = p.Price,
                DiscountPrice = p.DiscountPrice,
                ImageName = p.ImageName,
                CreatedAt = p.CreatedAt,
                AverageRating = p.AverageRating,
                ReviewCount = p.ReviewCount
            }).ToList();

            var model = new HomeModel
            {
                Products = productItems
            };

            return View(model);
        }
    }
}