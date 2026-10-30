using System;
using System.Collections.Generic;
using System.Text;

namespace MaleFashion.Application.DTOs
{
    public class OrderItemDto
    {
        public Guid Id { get; set; }
        public Guid? ProductVariantId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string Size { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public string SKU { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal LineTotal => UnitPrice * Quantity;
    }
}
