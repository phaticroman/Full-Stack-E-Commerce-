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
    public class CategoryAddCommandHandler : ICommandHandler<CategoryAddCommand, Guid>
    {
        private readonly IApplicationUnitOfwork _unitOfWork;

        public CategoryAddCommandHandler(IApplicationUnitOfwork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> Handle(CategoryAddCommand command, CancellationToken cancellationToken)
        {
            var slug = SlugHelper.GenerateSlug(command.Name);

            var isDuplicate = await _unitOfWork.CategoryRepository.IsDuplicateCategorySlug(slug, null, cancellationToken);
            if (isDuplicate)
                throw new DuplicateDataException("Category slug is duplicate");

            var category = Category.Create(
                name: command.Name,
                slug: slug,
                description: command.Description,
                imageName: command.ImageName,
                displayOrder: command.DisplayOrder,
                isActive: command.IsActive);

            await _unitOfWork.CategoryRepository.AddAsync(category, cancellationToken);
            await _unitOfWork.SaveAsync(cancellationToken);

            return category.Id;
        }
    }
}
