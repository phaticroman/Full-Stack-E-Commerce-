using System;
using System.Collections.Generic;
using System.Text;

namespace MaleFashion.Application.DTOs
{
    public class ProductVariantDto
    {
        public string Size { get; set; } = null!;
        public string Color { get; set; } = null!;
        public string SKU { get; set; } = null!;
        public int StockQuantity { get; set; }
    }
}
