using System;
using System.Collections.Generic;
using System.Text;

namespace MaleFashion.Application.DTOs
{
    public class CartDto
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public bool IsEmpty { get; set; }
        public int TotalItems { get; set; }
        public decimal SubTotal { get; set; }
        public IList<CartItemDto> Items { get; set; } = new List<CartItemDto>();
    }
}
