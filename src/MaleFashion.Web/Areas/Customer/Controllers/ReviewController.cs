using Cortex.Mediator;
using MaleFashion.Application.Exceptions;
using MaleFashion.Application.Features.Reviews.Command;
using MaleFashion.Infrastructure.Extensions;
using MaleFashion.Web.Areas.Admin.Models;
using MaleFashion.Web.Areas.Customer.Models;
using MaleFashion.Web.Codes;
using MapsterMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MaleFashion.Web.Areas.Customer.Controllers
{
    [Area("Customer"), Authorize(Roles = "Member,Admin")]
    public class ReviewsController : Controller
    {
        private readonly ILogger<ReviewsController> _logger;
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public ReviewsController(ILogger<ReviewsController> logger, IMediator mediator, IMapper mapper)
        {
            _logger = logger;
            _mediator = mediator;
            _mapper = mapper;
        }

        private Guid CurrentCustomerId =>
            Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = "Member")]
        public async Task<IActionResult> Add(ReviewModel model, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                TempData.Put(Constants.ResponseTempKey,
                    new ResponseModel { Message = "Please provide a valid rating (1-5).", Type = ResponseTypes.Danger });
                return RedirectToAction("Details", "Shop", new { id = model.ProductId });
            }

            try
            {
                var command = new AddReviewCommand
                {
                    ProductId = model.ProductId,
                    CustomerId = CurrentCustomerId,
                    Rating = model.Rating,
                    Comment = model.Comment
                };

                await _mediator.SendCommandAsync(command, cancellationToken);

                TempData.Put(Constants.ResponseTempKey,
                    new ResponseModel { Message = "Thank you for your review!", Type = ResponseTypes.Success });
            }
            catch (DuplicateDataException oex)
            {
                TempData.Put(Constants.ResponseTempKey,
                    new ResponseModel { Message = oex.Message, Type = ResponseTypes.Danger });
            }
            catch (DomainException dex)
            {
                TempData.Put(Constants.ResponseTempKey,
                    new ResponseModel { Message = dex.Message, Type = ResponseTypes.Danger });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to add review.");
                TempData.Put(Constants.ResponseTempKey,
                    new ResponseModel { Message = "Failed to submit review.", Type = ResponseTypes.Danger });
            }

            return RedirectToAction("Details", "Shop", new { id = model.ProductId });
        }

        [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = "Member")]
        public async Task<IActionResult> Update(ReviewModel model, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                TempData.Put(Constants.ResponseTempKey,
                    new ResponseModel { Message = "Please provide valid review details.", Type = ResponseTypes.Danger });
                return RedirectToAction("Details", "Shop", new { id = model.ProductId });
            }

            try
            {
                var command = new UpdateReviewCommand
                {
                    Id = model.Id,
                    CustomerId = CurrentCustomerId,
                    Rating = model.Rating,
                    Comment = model.Comment
                };

                await _mediator.SendCommandAsync(command, cancellationToken);

                TempData.Put(Constants.ResponseTempKey,
                    new ResponseModel { Message = "Review updated successfully.", Type = ResponseTypes.Success });
            }
            catch (UnauthorizedAccessException)
            {
                TempData.Put(Constants.ResponseTempKey,
                    new ResponseModel { Message = "You cannot edit this review.", Type = ResponseTypes.Danger });
            }
            catch (DomainException dex)
            {
                TempData.Put(Constants.ResponseTempKey,
                    new ResponseModel { Message = dex.Message, Type = ResponseTypes.Danger });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update review.");
                TempData.Put(Constants.ResponseTempKey,
                    new ResponseModel { Message = "Failed to update review.", Type = ResponseTypes.Danger });
            }

            return RedirectToAction("Details", "Shop", new { id = model.ProductId });
        }

        [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = "Member,Admin")]
        public async Task<IActionResult> Delete(Guid id, Guid productId, CancellationToken cancellationToken)
        {
            try
            {
                var isAdmin = User.IsInRole("Admin");

                await _mediator.SendCommandAsync(new DeleteReviewCommand
                {
                    Id = id,
                    CustomerId = CurrentCustomerId,
                }, cancellationToken);

                TempData.Put(Constants.ResponseTempKey,
                    new ResponseModel { Message = "Review deleted successfully.", Type = ResponseTypes.Success });
            }
            catch (UnauthorizedAccessException)
            {
                TempData.Put(Constants.ResponseTempKey,
                    new ResponseModel { Message = "You cannot delete this review.", Type = ResponseTypes.Danger });
            }
            catch (DomainException dex)
            {
                TempData.Put(Constants.ResponseTempKey,
                    new ResponseModel { Message = dex.Message, Type = ResponseTypes.Danger });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete review.");
                TempData.Put(Constants.ResponseTempKey,
                    new ResponseModel { Message = "Failed to delete review.", Type = ResponseTypes.Danger });
            }

            return RedirectToAction("Details", "Shop", new { id = productId });
        }
    }
}
