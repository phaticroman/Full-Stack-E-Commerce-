namespace MaleFashion.Web.Areas.Customer.Models
{
    public class HomeModel
    {
        public IList<HomeProductItemModel> Products { get; set; } = new List<HomeProductItemModel>();
    }

    public class HomeProductItemModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string Slug { get; set; } = null!;
        public decimal Price { get; set; }
        public decimal? DiscountPrice { get; set; }
        public string ImageName { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public double AverageRating { get; set; } = 5.0;
        public int ReviewCount { get; set; } = 0;
    }
}