using Cortex.Mediator.Commands;
using MaleFashion.Application.Contracts;
using MaleFashion.Application.Exceptions;
using MaleFashion.Domain.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaleFashion.Application.Features.Products.Command
{
    public class ProductUpdateCommandHandler : ICommandHandler<ProductUpdateCommand>
    {
        private readonly IApplicationUnitOfwork _unitOfWork;

        public ProductUpdateCommandHandler(IApplicationUnitOfwork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(ProductUpdateCommand command, CancellationToken cancellationToken)
        {
            var product = await _unitOfWork.ProductRepository.GetByIdAsync(command.Id, cancellationToken);
            if (product == null)
                throw new NotFoundException($"Product with ID '{command.Id}' was not found.");

            var isDuplicateName = await _unitOfWork.ProductRepository.IsDuplicateProductNameAsync(command.Name, command.Id, cancellationToken);
            if (isDuplicateName)
                throw new DuplicateDataException($"Product with name '{command.Name}' already exists.");

            var slug = SlugHelper.GenerateSlug(command.Name);

            product.UpdateDetails(
                name: command.Name,
                slug: slug,
                description: command.Description,
                price: command.Price,
                discountPrice: command.DiscountPrice,
                brand: command.Brand,
                categoryId: command.CategoryId);

            if (!string.IsNullOrWhiteSpace(command.ImageName))
            {
                product.ChangeMainImage(command.ImageName);
            }

            if (command.IsActive)
                product.Activate();
            else
                product.Deactivate();

            await _unitOfWork.SaveAsync(cancellationToken);
        }
    }
}
