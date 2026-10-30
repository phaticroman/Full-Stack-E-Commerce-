using Cortex.Mediator.Commands;
using MaleFashion.Application.Contracts;
using MaleFashion.Application.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaleFashion.Application.Features.ProductVariants.Command
{
    public class ProductVariantUpdateCommandHandler : ICommandHandler<ProductVariantUpdateCommand>
    {
        private readonly IApplicationUnitOfwork _unitOfWork;

        public ProductVariantUpdateCommandHandler(IApplicationUnitOfwork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(ProductVariantUpdateCommand command, CancellationToken cancellationToken)
        {
            var product = await _unitOfWork.ProductRepository.GetByIdWithDetailsAsync(command.ProductId, cancellationToken);
            if (product == null)
                throw new NotFoundException($"Product with ID '{command.ProductId}' was not found.");

            var isDuplicateSku = await _unitOfWork.ProductRepository.IsDuplicateSkuAsync(command.SKU, command.Id, cancellationToken);
            if (isDuplicateSku)
                throw new DuplicateDataException($"SKU '{command.SKU}' is already in use.");

            product.UpdateVariant(
                variantId: command.Id,
                sku: command.SKU,
                size: command.Size,
                color: command.Color,
                stockQuantity: command.StockQuantity);

            await _unitOfWork.SaveAsync(cancellationToken);
        }
    }
}
