using Cortex.Mediator;
using MaleFashion.Application.Exceptions;
using MaleFashion.Application.Features.Carts.Command;
using MaleFashion.Application.Features.Carts.Query;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MaleFashion.Web.Areas.Customer.Controllers
{
    [Area("Customer"), Authorize(Roles = "Member")]
    public class CartController : Controller
    {
        private readonly IMediator _mediator;
        private readonly ILogger<CartController> _logger;

        public CartController(IMediator mediator, ILogger<CartController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        private Guid CurrentCustomerId =>
            Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        [HttpGet]
        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var cart = await _mediator.SendQueryAsync(new GetCartByCustomerIdQuery(CurrentCustomerId), cancellationToken);
            return View(cart);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(Guid productVariantId, int quantity = 1, CancellationToken cancellationToken = default)
        {
            try
            {
                var command = new AddToCartCommand
                {
                    CustomerId = CurrentCustomerId,
                    ProductVariantId = productVariantId,
                    Quantity = quantity
                };

                await _mediator.SendCommandAsync(command, cancellationToken);

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    var count = await _mediator.SendQueryAsync(new GetCartItemCountQuery(CurrentCustomerId), cancellationToken);
                    return Json(new { success = true, message = "Item added to cart successfully.", totalItems = count });
                }

                return RedirectToAction(nameof(Index));
            }
            catch (DomainException dex)
            {
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    return Json(new { success = false, message = dex.Message });

                TempData["ErrorMessage"] = dex.Message;
                return Redirect(Request.Headers["Referer"].ToString() ?? "/Shop");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to add item to cart.");
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    return Json(new { success = false, message = "Unable to add product to cart." });

                TempData["ErrorMessage"] = "Unable to add product to cart.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateQuantity([FromBody] UpdateCartItemRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var command = new UpdateCartItemCommand
                {
                    CustomerId = CurrentCustomerId,
                    CartItemId = request.CartItemId,
                    Quantity = request.Quantity
                };

                var updatedCart = await _mediator.SendCommandAsync(command, cancellationToken);
                var updatedItem = updatedCart.Items.FirstOrDefault(i => i.Id == request.CartItemId);

                return Json(new
                {
                    success = true,
                    cartItemId = request.CartItemId,
                    itemQuantity = updatedItem?.Quantity ?? 0,
                    itemTotal = (updatedItem?.TotalPrice ?? 0m).ToString("C"),
                    subTotal = updatedCart.SubTotal.ToString("C"),
                    totalItems = updatedCart.TotalItems,
                    isEmpty = updatedCart.IsEmpty
                });
            }
            catch (DomainException dex)
            {
                return Json(new { success = false, message = dex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update item quantity.");
                return Json(new { success = false, message = "Error updating quantity." });
            }
        }
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateQuantity(Guid cartItemId, int quantity, CancellationToken cancellationToken)
        {
            try
            {
                await _mediator.SendCommandAsync(new UpdateCartItemCommand
                {
                    CustomerId = CurrentCustomerId,
                    CartItemId = cartItemId,
                    Quantity = quantity
                }, cancellationToken);
            }
            catch (DomainException dex)
            {
                TempData["ErrorMessage"] = dex.Message;
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Could not update item quantity.";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Remove(Guid cartItemId, CancellationToken cancellationToken)
        {
            try
            {
                var updatedCart = await _mediator.SendCommandAsync(
                    new RemoveFromCartCommand(CurrentCustomerId, cartItemId), cancellationToken);

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new
                    {
                        success = true,
                        subTotal = updatedCart.SubTotal.ToString("C"),
                        totalItems = updatedCart.TotalItems,
                        isEmpty = updatedCart.IsEmpty
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to remove item from cart.");
                TempData["ErrorMessage"] = "Could not remove item from cart.";
            }

            return RedirectToAction(nameof(Index));
        }
        

        [HttpGet]
        public async Task<IActionResult> GetCount(CancellationToken cancellationToken)
        {
            var count = await _mediator.SendQueryAsync(new GetCartItemCountQuery(CurrentCustomerId), cancellationToken);
            return Json(new { count });
        }
    }

    public class UpdateCartItemRequest
    {
        public Guid CartItemId { get; set; }
        public int Quantity { get; set; }
    }

    public class RemoveCartItemRequest
    {
        public Guid CartItemId { get; set; }
    }
}
