using System;
using System.Collections.Generic;
using System.Text;

namespace MaleFashion.Application.DTOs
{
    public class ReviewDto
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public Guid CustomerId { get; set; }
        public string? CustomerName { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
