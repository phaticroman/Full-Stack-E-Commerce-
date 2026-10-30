using MaleFashion.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaleFashion.Application.DTOs
{
    public class AdminOrderDetailsDto
    {
        public Guid Id { get; set; }
        public DateTime OrderDate { get; set; }
        public OrderStatus Status { get; set; }
        public decimal SubTotal { get; set; }
        public decimal ShippingFee { get; set; }
        public decimal Discount { get; set; }
        public decimal Total { get; set; }

        public string ShippingFullName { get; set; } = string.Empty;
        public string ShippingEmail { get; set; } = string.Empty;
        public string ShippingPhoneNumber { get; set; } = string.Empty;
        public string ShippingAddress { get; set; } = string.Empty;
        public string? OrderNotes { get; set; }

        public IList<AdminOrderItemDto> Items { get; set; } = new List<AdminOrderItemDto>();
    }
}
