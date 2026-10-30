using Cortex.Mediator.Commands;
using MaleFashion.Application.Contracts;
using MaleFashion.Application.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaleFashion.Application.Features.ProductVariants.Command
{
    public class ProductVariantAddCommandHandler : ICommandHandler<ProductVariantAddCommand, Guid>
    {
        private readonly IApplicationUnitOfwork _unitOfWork;

        public ProductVariantAddCommandHandler(IApplicationUnitOfwork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> Handle(ProductVariantAddCommand command, CancellationToken cancellationToken)
        {
            var product = await _unitOfWork.ProductRepository.GetByIdWithDetailsAsync(command.ProductId, cancellationToken);
            if (product == null)
                throw new NotFoundException($"Product with ID '{command.ProductId}' was not found.");

            var isDuplicateSku = await _unitOfWork.ProductRepository.IsDuplicateSkuAsync(command.SKU, null, cancellationToken);
            if (isDuplicateSku)
                throw new DuplicateDataException($"SKU '{command.SKU}' is already in use.");

            var variant = product.AddVariant(
                sku: command.SKU,
                size: command.Size,
                color: command.Color,
                stockQuantity: command.StockQuantity);

            await _unitOfWork.SaveAsync(cancellationToken);
            return variant.Id;
        }
    }
}
