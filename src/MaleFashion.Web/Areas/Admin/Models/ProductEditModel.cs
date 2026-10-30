using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace MaleFashion.Web.Areas.Admin.Models
{
    public class ProductEditModel
    {
        [Required]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Product name is required.")]
        [StringLength(200, ErrorMessage = "Product name cannot exceed 200 characters.")]
        public string Name { get; set; } = null!;

        public string Slug { get; set; } = null!;

        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters.")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Price is required.")]
        [Range(0.01, 1000000.00, ErrorMessage = "Price must be greater than 0.")]
        public decimal Price { get; set; }

        [Range(0.01, 1000000.00, ErrorMessage = "Discount price must be greater than 0.")]
        public decimal? DiscountPrice { get; set; }

        [Required(ErrorMessage = "Brand name is required.")]
        [StringLength(100, ErrorMessage = "Brand cannot exceed 100 characters.")]
        public string Brand { get; set; } = null!;

        [Required(ErrorMessage = "Please select a category.")]
        [Display(Name = "Category")]
        public Guid CategoryId { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; }

        public string? CurrentMainImage { get; set; }

        [Display(Name = "Change Main Image")]
        public IFormFile? MainImageFile { get; set; }

        public IList<ProductImageItemModel> Images { get; set; } = new List<ProductImageItemModel>();
        public IList<ProductVariantItemModel> Variants { get; set; } = new List<ProductVariantItemModel>();

        public IEnumerable<SelectListItem> Categories { get; set; } = new List<SelectListItem>();
    }
}
