using MaleFashion.Domain.Entites;

namespace MaleFashion.Web.Areas.Customer.Models
{
    public class ShopIndexViewModel
    {
        public IList<Product> Products { get; set; } = new List<Product>();
        public int TotalItems { get; set; }
        public int FilteredCount { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)FilteredCount / (Filter.PageSize <= 0 ? 9 : Filter.PageSize));

        public ShopFilterModel Filter { get; set; } = new();

        public IList<Category> Categories { get; set; } = new List<Category>();
        public Dictionary<Guid, int> CategoryCounts { get; set; } = new();
        public IList<string> AvailableSizes { get; set; } = new List<string>();
        public IList<string> AvailableColors { get; set; } = new List<string>();
    }
}
