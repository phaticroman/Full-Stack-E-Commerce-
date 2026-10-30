using System;
using System.Collections.Generic;
using System.Text;

namespace MaleFashion.Application.DTOs
{
    public class HomeProductDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string Slug { get; set; } = null!;
        public decimal Price { get; set; }
        public decimal? DiscountPrice { get; set; }
        public string ImageName { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public double AverageRating { get; set; }
        public int ReviewCount { get; set; }
    }
}
