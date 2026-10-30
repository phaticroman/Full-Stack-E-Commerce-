using MaleFashion.Domain.Contracts;
using MaleFashion.Domain.Exceptions;

namespace MaleFashion.Domain.Entites
{
    public class ProductImage
    {
        public Guid Id { get; private set; }
        public Guid ProductId { get; private set; }
        public Product? Product { get; private set; }
        public string ImageName { get; private set; } = null!;
        public int DisplayOrder { get; private set; }

        private ProductImage() { } 

        internal ProductImage(Guid id, Guid productId, string imageName, int displayOrder)
        {
            if (string.IsNullOrWhiteSpace(imageName))
                throw new DomainException("Image name cannot be empty.");

            Id = id;
            ProductId = productId;
            ImageName = imageName;
            DisplayOrder = displayOrder;
        }

        internal void SetDisplayOrder(int displayOrder) => DisplayOrder = displayOrder;
    }
}