using Cortex.Mediator.Commands;
using MaleFashion.Application.Contracts;
using MaleFashion.Application.Exceptions;
using MaleFashion.Domain.Entites;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaleFashion.Application.Features.Reviews.Command
{
    public class AddReviewCommandHandler : ICommandHandler<AddReviewCommand>
    {
        private readonly IApplicationUnitOfwork _unitOfWork;

        public AddReviewCommandHandler(IApplicationUnitOfwork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(AddReviewCommand command, CancellationToken cancellationToken = default)
        {

            var product = await _unitOfWork.ProductRepository.GetByIdAsync(command.ProductId, cancellationToken);
            if (product == null)
            {
                throw new NotFoundException($"Product with ID '{command.ProductId}' was not found.");
            }

            var alreadyReviewed = await _unitOfWork.ReviewRepository.HasCustomerReviewedProductAsync(
                command.ProductId,
                command.CustomerId,
                cancellationToken);

            if (alreadyReviewed)
            {
                throw new DuplicateDataException("You have already submitted a review for this product.");
            }

            var review = Review.Create(
                command.ProductId,
                command.CustomerId,
                command.Rating,
                command.Comment);

            await _unitOfWork.ReviewRepository.AddAsync(review, cancellationToken);
            await _unitOfWork.SaveAsync(cancellationToken);

            command.Id = review.Id;
        }
    }
}
