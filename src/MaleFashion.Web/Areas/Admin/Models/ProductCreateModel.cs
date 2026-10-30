using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace MaleFashion.Web.Areas.Admin.Models
{
    public class ProductCreateModel
    {
        [Required(ErrorMessage = "Product name is required.")]
        [StringLength(200, ErrorMessage = "Product name cannot exceed 200 characters.")]
        public string Name { get; set; } = null!;

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
        public bool IsActive { get; set; } = true;

        [Required(ErrorMessage = "Main image is required.")]
        [Display(Name = "Main Image")]
        public IFormFile MainImageFile { get; set; } = null!;

        [Display(Name = "Gallery Images")]
        public IList<IFormFile>? GalleryFiles { get; set; }

        public string? VariantsJson { get; set; }

        public IEnumerable<SelectListItem> Categories { get; set; } = new List<SelectListItem>();
    }
}
