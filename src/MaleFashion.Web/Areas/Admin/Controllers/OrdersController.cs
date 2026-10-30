using Cortex.Mediator;
using MaleFashion.Application.Features.Orders.Command;
using MaleFashion.Application.Features.Orders.Query;
using MaleFashion.Domain.Enums;
using MaleFashion.Infrastructure.Extensions;
using MaleFashion.Web.Areas.Admin.Models;
using MaleFashion.Web.Codes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MaleFashion.Web.Areas.Admin.Controllers
{
    [Area("Admin"), Authorize(Roles = "Admin")]
    public class OrdersController : Controller
    {
        private readonly IMediator _mediator;

        public OrdersController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpGet]
        public async Task<IActionResult> Index(
            string? search,
            OrderStatus? status,
            int page = 1,
            CancellationToken cancellationToken = default)
        {
            var query = new GetPagedOrdersQuery
            {
                SearchTerm = search,
                Status = status,
                PageIndex = page,
                PageSize = 10 
            };

            var result = await _mediator.SendQueryAsync(query, cancellationToken);

            ViewBag.CurrentSearch = search;
            ViewBag.CurrentStatus = status;

            return View(result);
        }
        [HttpGet]
        public async Task<IActionResult> Details(Guid id, CancellationToken cancellationToken)
        {
            var order = await _mediator.SendQueryAsync(new GetOrderDetailsForAdminQuery(id), cancellationToken);
            if (order == null)
            {
                TempData.Put(Constants.ResponseTempKey,
                    new ResponseModel { Message = "Order not found.", Type = ResponseTypes.Danger });
                return RedirectToAction(nameof(Index));
            }

            return View(order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(Guid orderId, OrderStatus newStatus, CancellationToken cancellationToken)
        {
            try
            {
                await _mediator.SendCommandAsync(new UpdateOrderStatusCommand(orderId, newStatus), cancellationToken);

                TempData.Put(Constants.ResponseTempKey,
                    new ResponseModel { Message = $"Order status successfully updated to {newStatus}.", Type = ResponseTypes.Success });
            }
            catch (Exception ex)
            {
                TempData.Put(Constants.ResponseTempKey,
                    new ResponseModel { Message = ex.Message ?? "Failed to update order status.", Type = ResponseTypes.Danger });
            }

            return RedirectToAction(nameof(Details), new { id = orderId });
        }
    }
}
