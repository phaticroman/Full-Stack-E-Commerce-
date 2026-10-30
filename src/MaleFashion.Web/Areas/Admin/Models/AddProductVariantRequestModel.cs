namespace MaleFashion.Web.Areas.Admin.Models
{
    public class AddProductVariantRequestModel
    {
        public Guid ProductId { get; set; }
        public string Size { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public string SKU { get; set; } = string.Empty;
        public int StockQuantity { get; set; }
    }
}
