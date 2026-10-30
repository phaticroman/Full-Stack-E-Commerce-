using Cortex.Mediator.Commands;
using MaleFashion.Application.Contracts;
using MaleFashion.Application.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaleFashion.Application.Features.ProductImages.Command
{
    public class ProductImageReorderCommandHandler : ICommandHandler<ProductImageReorderCommand>
    {
        private readonly IApplicationUnitOfwork _unitOfWork;

        public ProductImageReorderCommandHandler(IApplicationUnitOfwork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(ProductImageReorderCommand command, CancellationToken cancellationToken)
        {
            var product = await _unitOfWork.ProductRepository.GetByImageIdWithDetailsAsync(command.ImageId, cancellationToken);
            if (product == null)
                throw new NotFoundException($"Image with ID '{command.ImageId}' was not found.");

            product.ReorderImage(command.ImageId, command.NewDisplayOrder);

            await _unitOfWork.SaveAsync(cancellationToken);
        }
    }
}
