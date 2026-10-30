using Cortex.Mediator.Queries;
using MaleFashion.Application.Contracts;
using MaleFashion.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaleFashion.Application.Features.Orders.Query
{
    public class GetOrderDetailsForAdminQueryHandler : IQueryHandler<GetOrderDetailsForAdminQuery, AdminOrderDetailsDto?>
    {
        private readonly IApplicationUnitOfwork _unitOfWork;

        public GetOrderDetailsForAdminQueryHandler(IApplicationUnitOfwork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<AdminOrderDetailsDto?> Handle(GetOrderDetailsForAdminQuery query, CancellationToken cancellationToken)
        {
            var order = await _unitOfWork.OrderRepository.GetByIdWithItemsAsync(query.OrderId, cancellationToken);
            if (order == null) return null;

            return new AdminOrderDetailsDto
            {
                Id = order.Id,
                OrderDate = order.OrderDate,
                Status = order.Status,
                SubTotal = order.Subtotal, 
                ShippingFee = order.ShippingFee,
                Discount = order.DiscountAmount, 
                Total = order.Total,
                ShippingFullName = order.ShippingFullName,
                ShippingEmail = order.ShippingEmail,
                ShippingPhoneNumber = order.ShippingPhone,
                ShippingAddress = $"{order.ShippingCity}, Postal Code: {order.ShippingPostalCode}",
                OrderNotes = null,
                Items = order.Items.Select(item => new AdminOrderItemDto
                {
                    Id = item.Id,
                    ProductName = item.ProductName,
                    VariantInfo = $"Size: {item.Size} | Color: {item.Color}",
                    SKU = item.SKU,
                    UnitPrice = item.UnitPrice,
                    Quantity = item.Quantity
                }).ToList()
            };
        }
    }
}
