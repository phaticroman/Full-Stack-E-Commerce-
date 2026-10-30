namespace MaleFashion.Web.Areas.Customer.Models
{
    public class ProductReviewItemModel
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public string CustomerName { get; set; } = "Anonymous";
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsOwner { get; set; }
    }
}
