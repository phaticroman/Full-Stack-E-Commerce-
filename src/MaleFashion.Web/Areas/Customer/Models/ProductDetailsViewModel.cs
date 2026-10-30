using MaleFashion.Domain.Entites;

namespace MaleFashion.Web.Areas.Customer.Models
{
    public class ProductDetailsViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string Slug { get; set; } = null!;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public decimal? DiscountPrice { get; set; }
        public string Brand { get; set; } = null!;
        public string ImageName { get; set; } = null!;
        public string CategoryName { get; set; } = "Uncategorized";

        public double AverageRating { get; set; } = 5.0;
        public int ReviewCount { get; set; } = 0;
        public bool HasCustomerReviewed { get; set; } = false;

        public IList<ProductImage> Images { get; set; } = new List<ProductImage>();
        public IList<ProductVariant> Variants { get; set; } = new List<ProductVariant>();
        public IList<Product> RelatedProducts { get; set; } = new List<Product>();
        public IList<ProductReviewItemModel> Reviews { get; set; } = new List<ProductReviewItemModel>();
    }
}
