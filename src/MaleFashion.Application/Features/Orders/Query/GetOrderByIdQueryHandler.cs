using Cortex.Mediator.Queries;
using MaleFashion.Application.Contracts;
using MaleFashion.Application.DTOs;
using MaleFashion.Application.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaleFashion.Application.Features.Orders.Query
{
    public class GetOrderByIdQueryHandler : IQueryHandler<GetOrderByIdQuery, OrderDto>
    {
        private readonly IApplicationUnitOfwork _unitOfWork;

        public GetOrderByIdQueryHandler(IApplicationUnitOfwork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<OrderDto> Handle(GetOrderByIdQuery query, CancellationToken cancellationToken)
        {
            var order = await _unitOfWork.OrderRepository.GetByIdWithItemsAsync(query.OrderId, cancellationToken);
            if (order == null || order.CustomerId != query.CustomerId)
                throw new NotFoundException("Order not found.");

            return new OrderDto
            {
                Id = order.Id,
                CustomerId = order.CustomerId,
                OrderDate = order.OrderDate,
                Status = order.Status,
                ShippingFullName = order.ShippingFullName,
                ShippingPhone = order.ShippingPhone,
                ShippingEmail = order.ShippingEmail,
                ShippingCity = order.ShippingCity,
                ShippingPostalCode = order.ShippingPostalCode,
                Subtotal = order.Subtotal,
                ShippingFee = order.ShippingFee,
                DiscountAmount = order.DiscountAmount,
                Total = order.Total,
                Items = order.Items.Select(i => new OrderItemDto
                {
                    Id = i.Id,
                    ProductVariantId = i.ProductVariantId,
                    ProductName = i.ProductName,
                    Size = i.Size,
                    Color = i.Color,
                    SKU = i.SKU,
                    UnitPrice = i.UnitPrice,
                    Quantity = i.Quantity
                }).ToList()
            };
        }
    }
}
