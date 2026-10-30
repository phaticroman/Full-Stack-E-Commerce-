using MaleFashion.Application.Contracts;
using MaleFashion.Application.Contracts.Repositories;
using MaleFashion.Domain.Contracts;
using MaleFashion.Infrastructure.Data.Respositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaleFashion.Infrastructure.Data
{
    public class ApplicationUnitOfWork : UnitOfWork, IApplicationUnitOfwork
    {
        public ISqlUtility SqlUtility { get; private set; }
        public IProductRepository ProductRepository { get; private set; } 
        public ICategoryRepository CategoryRepository { get; private set; }
        public ICartRepository CartRepository { get; private set; }
        public IOrderRepository OrderRepository { get; private set; }
        public IReviewRepository ReviewRepository { get; private set; }

        public ApplicationUnitOfWork(ApplicationDbContext dbContext, IProductRepository productRepository,
            ICategoryRepository categoryRepository, 
            ICartRepository cartRepository,
            IOrderRepository orderRepository,
            IReviewRepository reviewRepository) : base(dbContext)
        {
            
            {
                SqlUtility = new SqlUtility(dbContext.Database.GetDbConnection());
                ProductRepository = productRepository;
                CategoryRepository = categoryRepository;
                CartRepository = cartRepository;
                OrderRepository = orderRepository;
                ReviewRepository = reviewRepository;
            }
        }
    }
}
