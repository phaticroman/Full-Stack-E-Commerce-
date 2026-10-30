using MaleFashion.Application.Contracts.Repositories;
using MaleFashion.Domain.Contracts;

namespace MaleFashion.Application.Contracts
{
    public interface IApplicationUnitOfwork : IUnitOfWork
    {
        public IProductRepository ProductRepository { get; }
        public ICategoryRepository CategoryRepository { get; }
        public ICartRepository CartRepository { get; }
        public IOrderRepository OrderRepository { get; }
        public IReviewRepository ReviewRepository { get; }
    }
}
