using System;
using System.Collections.Generic;
using System.Text;

namespace MaleFashion.Application.DTOs
{
    public class ProductImageDto
    {
        public string ImageUrl { get; set; } = null!;
        public bool IsPrimary { get; set; }
        public int DisplayOrder { get; set; }
    }
}
