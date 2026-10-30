using Cortex.Mediator.Commands;
using MaleFashion.Application.Contracts;
using MaleFashion.Application.Exceptions;
using MaleFashion.Domain.Entites;
using MaleFashion.Domain.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaleFashion.Application.Features.Products.Command
{
    public class ProductAddCommandHandler : ICommandHandler<ProductAddCommand, Guid>
    {
        private readonly IApplicationUnitOfwork _unitOfWork;

        public ProductAddCommandHandler(IApplicationUnitOfwork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> Handle(ProductAddCommand command, CancellationToken cancellationToken)
        {
            var isDuplicateName = await _unitOfWork.ProductRepository.IsDuplicateProductNameAsync(command.Name, null, cancellationToken);
            if (isDuplicateName)
                throw new DuplicateDataException($"Product with name '{command.Name}' already exists.");

            var slug = SlugHelper.GenerateSlug(command.Name);

            var product = Product.Create(
                name: command.Name,
                slug: slug,
                description: command.Description,
                price: command.Price,
                discountPrice: command.DiscountPrice,
                brand: command.Brand,
                imageName: command.ImageName,
                categoryId: command.CategoryId,
                createdAtUtc: DateTime.UtcNow,
                isActive: command.IsActive);

            foreach (var variant in command.Variants)
            {
                var isSkuTaken = await _unitOfWork.ProductRepository.IsDuplicateSkuAsync(variant.SKU, null, cancellationToken);
                if (isSkuTaken)
                    throw new DuplicateDataException($"SKU '{variant.SKU}' is already in use.");

                product.AddVariant(
                    size: variant.Size,
                    color: variant.Color,
                    sku: variant.SKU,
                    stockQuantity: variant.StockQuantity);
            }

            foreach (var imageName in command.GalleryImages)
            {
                product.AddImage(imageName);
            }

            await _unitOfWork.ProductRepository.AddAsync(product, cancellationToken);
            await _unitOfWork.SaveAsync(cancellationToken);

            return product.Id;
        }
    }
}
