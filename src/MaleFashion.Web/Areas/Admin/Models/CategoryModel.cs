using System.ComponentModel.DataAnnotations;

namespace MaleFashion.Web.Areas.Admin.Models
{
    public class CategoryModel
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Category name is required.")]
        [StringLength(150, ErrorMessage = "Category name cannot exceed 150 characters.")]
        [Display(Name = "Category Name")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Slug")]
        public string? Slug { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        [Display(Name = "Description")]
        public string? Description { get; set; }

        [Display(Name = "Current Image")]
        public string? ImageName { get; set; }

        [Display(Name = "Upload Image")]
        public IFormFile? ImageFile { get; set; }

        [Display(Name = "Display Order")]
        public int DisplayOrder { get; set; }

        [Display(Name = "Active Status")]
        public bool IsActive { get; set; } = true;
    }
}
