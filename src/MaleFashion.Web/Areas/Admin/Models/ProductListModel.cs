using MaleFashion.Domain.Utilities;

namespace MaleFashion.Web.Areas.Admin.Models
{
    public class ProductListModel : DataTables
    {
        public Guid? CategoryId { get; set; }
    }
}
