using Cortex.Mediator.Commands;
using MaleFashion.Application.Contracts;
using MaleFashion.Application.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaleFashion.Application.Features.Reviews.Command
{
    public class ReviewUpdateCommandHandler : ICommandHandler<UpdateReviewCommand>
    {
        private readonly IApplicationUnitOfwork _unitOfWork;

        public ReviewUpdateCommandHandler(IApplicationUnitOfwork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(UpdateReviewCommand command, CancellationToken cancellationToken = default)
        {
            var review = await _unitOfWork.ReviewRepository.GetByIdAsync(command.Id, cancellationToken);
            if (review == null)
            {
                throw new DomainException("Review was not found.");
            }

            if (review.CustomerId != command.CustomerId)
            {
                throw new UnauthorizedAccessException("You cannot edit this review.");
            }

            review.Update(command.Rating, command.Comment);

            await _unitOfWork.SaveAsync(cancellationToken);
        }
    }
}
