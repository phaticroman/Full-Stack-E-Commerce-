using Cortex.Mediator.Queries;
using MaleFashion.Application.Contracts;
using MaleFashion.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaleFashion.Application.Features.Orders.Query
{
    public class GetCustomerOrdersQueryHandler : IQueryHandler<GetCustomerOrdersQuery, IList<OrderDto>>
    {
        private readonly IApplicationUnitOfwork _unitOfWork;

        public GetCustomerOrdersQueryHandler(IApplicationUnitOfwork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IList<OrderDto>> Handle(GetCustomerOrdersQuery query, CancellationToken cancellationToken)
        {
            var orders = await _unitOfWork.OrderRepository.GetOrdersByCustomerIdAsync(query.CustomerId, cancellationToken);

            return orders.Select(order => new OrderDto
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
            }).ToList();
        }
    }
}
