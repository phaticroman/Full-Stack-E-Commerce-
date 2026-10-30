using System.ComponentModel.DataAnnotations;

namespace MaleFashion.Web.Areas.Admin.Models
{
    public class ProductVariantItemModel
    {
        public Guid? Id { get; set; }

        [Required(ErrorMessage = "Size is required.")]
        [StringLength(20)]
        public string Size { get; set; } = null!;

        [Required(ErrorMessage = "Color is required.")]
        [StringLength(30)]
        public string Color { get; set; } = null!;

        [Required(ErrorMessage = "SKU is required.")]
        [StringLength(50)]
        public string SKU { get; set; } = null!;

        [Range(0, 100000, ErrorMessage = "Stock quantity must be non-negative.")]
        public int StockQuantity { get; set; }
    }
}
