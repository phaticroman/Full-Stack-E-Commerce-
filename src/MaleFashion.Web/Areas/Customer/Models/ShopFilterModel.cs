namespace MaleFashion.Web.Areas.Customer.Models
{
    public class ShopFilterModel
    {
        public Guid? CategoryId { get; set; }
        public string? SearchText { get; set; }
        public decimal? PriceFrom { get; set; }
        public decimal? PriceTo { get; set; }
        public string? Size { get; set; }
        public string? Color { get; set; }
        public string? SortBy { get; set; } = "newest";
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 9;
    }
}
