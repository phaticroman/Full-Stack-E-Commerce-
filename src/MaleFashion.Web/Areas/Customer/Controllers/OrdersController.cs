using Cortex.Mediator;
using MaleFashion.Application.Exceptions;
using MaleFashion.Application.Features.Carts.Query;
using MaleFashion.Application.Features.Orders.Command;
using MaleFashion.Application.Features.Orders.Query;
using MaleFashion.Web.Areas.Customer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MaleFashion.Web.Areas.Customer.Controllers
{
    [Area("Customer"), Authorize(Roles = "Member")]
    public class OrdersController : Controller
    {
        private readonly IMediator _mediator;
        private readonly ILogger<OrdersController> _logger;

        public OrdersController(IMediator mediator, ILogger<OrdersController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        private Guid CurrentCustomerId =>
            Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        [HttpGet]
        public async Task<IActionResult> Checkout(CancellationToken cancellationToken)
        {
            var cart = await _mediator.SendQueryAsync(new GetCartByCustomerIdQuery(CurrentCustomerId), cancellationToken);
            if (cart == null || cart.IsEmpty)
            {
                TempData["ErrorMessage"] = "Your cart is empty. Please add items before checking out.";
                return RedirectToAction("Index", "Cart", new { area = "Customer" });
            }

            var vm = new CheckoutViewModel
            {
                Cart = cart,
                Command = new CreateOrderCommand
                {
                    CustomerId = CurrentCustomerId,
                    ShippingEmail = User.FindFirstValue(ClaimTypes.Email) ?? string.Empty,
                    ShippingFullName = User.Identity?.Name ?? string.Empty
                }
            };

            return View(vm);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(CheckoutViewModel vm, CancellationToken cancellationToken)
        {
            vm.Command.CustomerId = CurrentCustomerId;

            if (!ModelState.IsValid)
            {

                vm.Cart = await _mediator.SendQueryAsync(new GetCartByCustomerIdQuery(CurrentCustomerId), cancellationToken);
                return View(vm);
            }

            try
            {
                var orderId = await _mediator.SendCommandAsync(vm.Command, cancellationToken);
                return RedirectToAction(nameof(OrderSuccess), new { id = orderId });
            }
            catch (DomainException dex)
            {
                ModelState.AddModelError(string.Empty, dex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to place order.");
                ModelState.AddModelError(string.Empty, "Unable to complete order. Please try again.");
            }

            vm.Cart = await _mediator.SendQueryAsync(new GetCartByCustomerIdQuery(CurrentCustomerId), cancellationToken);
            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> OrderSuccess(Guid id, CancellationToken cancellationToken)
        {
            try
            {
                var order = await _mediator.SendQueryAsync(new GetOrderByIdQuery(id, CurrentCustomerId), cancellationToken);
                return View(order);
            }
            catch
            {
                ViewBag.OrderId = id;
                return View();
            }
        }

        [HttpGet]
        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var orders = await _mediator.SendQueryAsync(new GetCustomerOrdersQuery(CurrentCustomerId), cancellationToken);
            return View(orders);
        }
        [HttpGet]
        public async Task<IActionResult> Details(Guid id, CancellationToken cancellationToken)
        {
            try
            {
                var order = await _mediator.SendQueryAsync(new GetOrderByIdQuery(id, CurrentCustomerId), cancellationToken);
                return View(order);
            }
            catch (NotFoundException)
            {
                TempData["ErrorMessage"] = "Order not found or you do not have permission to view it.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(Guid orderId, CancellationToken cancellationToken)
        {
            try
            {
                await _mediator.SendCommandAsync(new CancelOrderCommand(orderId, CurrentCustomerId), cancellationToken);
                TempData["SuccessMessage"] = "Order was successfully cancelled and stock has been restored.";
            }
            catch (DomainException dex)
            {
                TempData["ErrorMessage"] = dex.Message;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to cancel order {OrderId}", orderId);
                TempData["ErrorMessage"] = "Unable to cancel this order. Please try again.";
            }

            return RedirectToAction(nameof(Details), new { id = orderId });
        }
    }
}
