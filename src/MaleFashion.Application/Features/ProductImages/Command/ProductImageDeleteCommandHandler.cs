using Cortex.Mediator.Commands;
using MaleFashion.Application.Contracts;
using MaleFashion.Application.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaleFashion.Application.Features.ProductImages.Command
{
    public class ProductImageDeleteCommandHandler : ICommandHandler<ProductImageDeleteCommand, string>
    {
        private readonly IApplicationUnitOfwork _unitOfWork;

        public ProductImageDeleteCommandHandler(IApplicationUnitOfwork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<string> Handle(ProductImageDeleteCommand command, CancellationToken cancellationToken)
        {
            var product = await _unitOfWork.ProductRepository.GetByImageIdWithDetailsAsync(command.ImageId, cancellationToken);
            if (product == null)
                throw new NotFoundException($"Image with ID '{command.ImageId}' was not found.");

            var image = product.Images.FirstOrDefault(i => i.Id == command.ImageId)
                ?? throw new NotFoundException($"Image with ID '{command.ImageId}' was not found on the product.");

            var fileName = image.ImageName;

            product.RemoveImage(command.ImageId);

            await _unitOfWork.SaveAsync(cancellationToken);

            return fileName;
        }
    }
}
