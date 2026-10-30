using Cortex.Mediator.Commands;
using MaleFashion.Application.Contracts;
using MaleFashion.Application.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaleFashion.Application.Features.Categories.Command
{
    public class CategoryDeleteCommandHandler : ICommandHandler<CategoryDeleteCommand>
    {
        private readonly IApplicationUnitOfwork _unitOfWork;

        public CategoryDeleteCommandHandler(IApplicationUnitOfwork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(CategoryDeleteCommand command, CancellationToken cancellationToken)
        {
            var category = await _unitOfWork.CategoryRepository.GetByIdAsync(command.Id, cancellationToken);
            if (category == null)
                throw new NotFoundException($"Category with id '{command.Id}' was not found.");

            await _unitOfWork.CategoryRepository.RemoveAsync(category, cancellationToken);
            await _unitOfWork.SaveAsync(cancellationToken);
        }
    }
}
