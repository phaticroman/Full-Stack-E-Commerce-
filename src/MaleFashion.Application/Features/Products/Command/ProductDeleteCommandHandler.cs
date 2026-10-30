using Cortex.Mediator.Commands;
using MaleFashion.Application.Contracts;
using MaleFashion.Application.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaleFashion.Application.Features.Products.Command
{
    public class ProductDeleteCommandHandler : ICommandHandler<ProductDeleteCommand>
    {
        private readonly IApplicationUnitOfwork _unitOfWork;

        public ProductDeleteCommandHandler(IApplicationUnitOfwork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(ProductDeleteCommand command, CancellationToken cancellationToken)
        {
            var product = await _unitOfWork.ProductRepository.GetByIdWithDetailsAsync(command.Id, cancellationToken);
            if (product == null)
                throw new NotFoundException($"Product with ID '{command.Id}' was not found.");

            await _unitOfWork.ProductRepository.RemoveAsync(product, cancellationToken);
            await _unitOfWork.SaveAsync(cancellationToken);
        }
    }
}
