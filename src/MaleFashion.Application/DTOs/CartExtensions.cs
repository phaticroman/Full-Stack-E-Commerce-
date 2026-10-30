using MaleFashion.Domain.Entites;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaleFashion.Application.DTOs
{
    public static class CartExtensions
    {
        public static CartDto ToDto(this Cart cart)
        {
            var items = cart.Items.Select(i =>
            {
                var product = i.ProductVariant?.Product;
                var unitPrice = product != null
                    ? (product.DiscountPrice ?? product.Price)
                    : 0m;

                return new CartItemDto
                {
                    Id = i.Id,
                    ProductVariantId = i.ProductVariantId,
                    ProductName = product?.Name ?? "Product",
                    ProductSlug = product?.Slug ?? string.Empty,
                    ProductImage = product?.ImageName ?? string.Empty,
                    Size = i.ProductVariant?.Size ?? string.Empty,
                    Color = i.ProductVariant?.Color ?? string.Empty,
                    SKU = i.ProductVariant?.SKU ?? string.Empty,
                    UnitPrice = unitPrice,
                    Quantity = i.Quantity,
                    TotalPrice = unitPrice * i.Quantity,
                    AvailableStock = i.ProductVariant?.StockQuantity ?? 0
                };
            }).ToList();

            return new CartDto
            {
                Id = cart.Id,
                CustomerId = cart.CustomerId,
                IsEmpty = cart.IsEmpty,
                TotalItems = items.Sum(x => x.Quantity),
                SubTotal = items.Sum(x => x.TotalPrice),
                Items = items
            };
        }
    }
}
