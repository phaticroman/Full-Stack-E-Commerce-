using Cortex.Mediator.Commands;
using MaleFashion.Application.Contracts;
using MaleFashion.Application.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaleFashion.Application.Features.ProductVariants.Command
{
    public class ProductVariantDeleteCommandHandler : ICommandHandler<ProductVariantDeleteCommand>
    {
        private readonly IApplicationUnitOfwork _unitOfWork;

        public ProductVariantDeleteCommandHandler(IApplicationUnitOfwork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(ProductVariantDeleteCommand command, CancellationToken cancellationToken)
        {
            var product = await _unitOfWork.ProductRepository.GetByVariantIdWithDetailsAsync(command.Id, cancellationToken);
            if (product == null)
                throw new NotFoundException($"Variant with ID '{command.Id}' was not found.");

            product.RemoveVariant(command.Id);
            await _unitOfWork.SaveAsync(cancellationToken);
        }
    }
}
