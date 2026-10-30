using Cortex.Mediator.Commands;
using MaleFashion.Application.Contracts;
using MaleFashion.Application.Exceptions;
using MaleFashion.Domain.Entites;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaleFashion.Application.Features.Orders.Command
{
    public class CreateOrderCommandHandler : ICommandHandler<CreateOrderCommand, Guid>
    {
        private readonly IApplicationUnitOfwork _unitOfWork;

        public CreateOrderCommandHandler(IApplicationUnitOfwork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> Handle(CreateOrderCommand command, CancellationToken cancellationToken)
        {

            var cart = await _unitOfWork.CartRepository.GetByCustomerIdWithItemsAsync(command.CustomerId, cancellationToken);
            if (cart == null || cart.IsEmpty)
                throw new DomainException("Your cart is empty. Cannot place an order.");


            var order = Order.Create(
                command.CustomerId,
                command.ShippingFullName,
                command.ShippingPhone,
                command.ShippingEmail,
                command.ShippingCity,
                command.ShippingPostalCode);

            foreach (var cartItem in cart.Items)
            {
                var variant = cartItem.ProductVariant;
                var product = variant?.Product;

                if (variant == null || product == null)
                    throw new NotFoundException("One or more items in your cart are no longer available.");

                if (variant.StockQuantity < cartItem.Quantity)
                {
                    throw new DomainException(
                        $"Product '{product.Name}' ({variant.Size}/{variant.Color}) has insufficient stock ({variant.StockQuantity} available).");
                }

                // এখানে কারেকশন: পজিটিভ কোয়ান্টিটি পাঠাতে হবে
                variant.DeductStock(cartItem.Quantity);

                var unitPrice = product.DiscountPrice ?? product.Price;

                order.AddLineItem(
                    variant.Id,
                    product.Name,
                    variant.Size,
                    variant.Color,
                    variant.SKU,
                    unitPrice,
                    cartItem.Quantity);
            }

            // ৪. অর্ডার সেভ এবং কার্ট খালি করা
            await _unitOfWork.OrderRepository.AddAsync(order, cancellationToken);
            cart.Clear();

            await _unitOfWork.SaveAsync(cancellationToken);

            return order.Id;
        }
    }
}
