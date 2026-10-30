using MaleFashion.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaleFashion.Application.DTOs
{
    public class AdminOrderListItemDto
    {
        public Guid Id { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public decimal Total { get; set; }
        public OrderStatus Status { get; set; }
        public DateTime OrderDate { get; set; }
        public int ItemCount { get; set; }
    }
}
