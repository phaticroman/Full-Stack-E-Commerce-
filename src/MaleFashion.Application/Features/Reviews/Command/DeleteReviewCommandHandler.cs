using Cortex.Mediator.Commands;
using MaleFashion.Application.Contracts;
using MaleFashion.Application.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaleFashion.Application.Features.Reviews.Command
{
    public class DeleteReviewCommandHandler : ICommandHandler<DeleteReviewCommand>
    {
        private readonly IApplicationUnitOfwork _unitOfWork;

        public DeleteReviewCommandHandler(IApplicationUnitOfwork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(DeleteReviewCommand command, CancellationToken cancellationToken = default)
        {
            var review = await _unitOfWork.ReviewRepository.GetByIdAsync(command.Id, cancellationToken);
            if (review == null)
            {
                throw new NotFoundException($"Review with ID '{command.Id}' was not found.");
            }

            if (review.CustomerId != command.CustomerId)
            {
                throw new DomainException("You are not authorized to delete this review.");
            }

            await _unitOfWork.ReviewRepository.RemoveAsync(review, cancellationToken);
            await _unitOfWork.SaveAsync(cancellationToken);
        }
    }
}
