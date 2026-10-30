using Mapster.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using MaleFashion.Domain.Enums;

namespace MaleFashion.Application.DTOs
{
    public class RecentOrderDto
    {
        public Guid Id { get; set; }
        public string ShippingFullName { get; set; } = null!;
        public DateTime OrderDate { get; set; }
        public decimal Total { get; set; }
        public OrderStatus Status { get; set; }
    }
}
