using Cortex.Mediator.Commands;
using MaleFashion.Application.Contracts;
using MaleFashion.Application.Exceptions;
using MaleFashion.Domain.Entites;
using MaleFashion.Domain.Utilities;
using MapsterMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaleFashion.Application.Features.Categories.Command
{
    public class CategoryUpdateCommandHandler : ICommandHandler<CategoryUpdateCommand>
    {
        private readonly IApplicationUnitOfwork _unitOfWork;

        public CategoryUpdateCommandHandler(IApplicationUnitOfwork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(CategoryUpdateCommand command, CancellationToken cancellationToken)
        {
            var category = await _unitOfWork.CategoryRepository.GetByIdAsync(command.Id, cancellationToken);
            if (category == null)
                throw new NotFoundException($"Category with id '{command.Id}' was not found.");

            var slug = SlugHelper.GenerateSlug(command.Name);

            var isDuplicate = await _unitOfWork.CategoryRepository.IsDuplicateCategorySlug(slug, command.Id, cancellationToken);
            if (isDuplicate)
                throw new DuplicateDataException("Category slug is duplicate");

            category.UpdateDetails(
                name: command.Name,
                slug: slug,
                description: command.Description,
                displayOrder: command.DisplayOrder,
                isActive: command.IsActive);

            if (!string.IsNullOrWhiteSpace(command.ImageName))
            {
                category.ChangeImage(command.ImageName);
            }

            await _unitOfWork.SaveAsync(cancellationToken);
        }
    }
}
