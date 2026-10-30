using Cortex.Mediator.Commands;
using MaleFashion.Application.Contracts;
using MaleFashion.Application.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaleFashion.Application.Features.ProductImages.Command
{
    public class ProductImageAddCommandHandler : ICommandHandler<ProductImageAddCommand, Guid>
    {
        private readonly IApplicationUnitOfwork _unitOfWork;

        public ProductImageAddCommandHandler(IApplicationUnitOfwork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> Handle(ProductImageAddCommand command, CancellationToken cancellationToken)
        {
            var product = await _unitOfWork.ProductRepository.GetByIdWithDetailsAsync(command.ProductId, cancellationToken);
            if (product == null)
                throw new NotFoundException($"Product with ID '{command.ProductId}' was not found.");

            var image = product.AddImage(command.ImageName);

            await _unitOfWork.SaveAsync(cancellationToken);
            return image.Id;
        }
    }
}
