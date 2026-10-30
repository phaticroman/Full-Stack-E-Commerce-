namespace MaleFashion.Web.Areas.Admin.Models
{
    public class ProductImageItemModel
    {
        public Guid Id { get; set; }
        public string ImageName { get; set; } = null!;
        public int DisplayOrder { get; set; }
    }
}
