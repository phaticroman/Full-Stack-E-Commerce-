using System;
using System.Threading;
using System.Threading.Tasks;
using Autofac.Extras.Moq;
using MaleFashion.Application.Contracts;
using MaleFashion.Application.Contracts.Repositories;
using MaleFashion.Application.Exceptions;
using MaleFashion.Application.Features.Products.Command;
using MaleFashion.Domain.Entites;
using Moq;
using NUnit.Framework;
using Shouldly;

namespace MaleFashion.Application.Tests.Features.Products.Commands
{
    [TestFixture]
    public class ProductDeleteCommandHandlerTests
    {
        private AutoMock _mock;
        private ProductDeleteCommandHandler _handler;
        private Mock<IApplicationUnitOfwork> _mockUnitOfWork;
        private Mock<IProductRepository> _mockProductRepository;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _mock = AutoMock.GetLoose();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            _mock?.Dispose();
        }

        [SetUp]
        public void Setup()
        {
            _mockUnitOfWork = _mock.Mock<IApplicationUnitOfwork>();
            _mockProductRepository = _mock.Mock<IProductRepository>();
            _handler = _mock.Create<ProductDeleteCommandHandler>();
        }

        [TearDown]
        public void TearDown()
        {
            _mockProductRepository?.Reset();
            _mockUnitOfWork?.Reset();
        }

        [Test]
        public async Task Handle_ProductNotFound_ThrowsNotFoundException()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var command = new ProductDeleteCommand(productId);

            _mockUnitOfWork.SetupGet(x => x.ProductRepository)
                .Returns(_mockProductRepository.Object)
                .Verifiable();

            // আপনার হ্যান্ডলার GetByIdWithDetailsAsync কল করে
            _mockProductRepository.Setup(x => x.GetByIdWithDetailsAsync(productId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Product?)null)
                .Verifiable();

            // Act & Assert
            var ex = await Should.ThrowAsync<NotFoundException>(async () =>
            {
                await _handler.Handle(command, default);
            });

            this.ShouldSatisfyAllConditions(
                () => ex.Message.ShouldContain(productId.ToString()),
                () => _mockProductRepository.VerifyAll(),
                () => _mockUnitOfWork.Verify(x => x.SaveAsync(It.IsAny<CancellationToken>()), Times.Never)
            );
        }

        [Test]
        public async Task Handle_ProductExists_RemovesProductAndSaves()
        {
            // Arrange
            var product = Product.Create(
                name: "Slim Fit Cotton Shirt",
                slug: "slim-fit-cotton-shirt",
                description: "Premium casual cotton shirt",
                price: 35.00m,
                discountPrice: null,
                brand: "MaleFashion",
                imageName: "shirt.jpg",
                categoryId: Guid.NewGuid(),
                createdAtUtc: DateTime.UtcNow,
                isActive: true
            );

            var command = new ProductDeleteCommand(product.Id);

            _mockUnitOfWork.SetupGet(x => x.ProductRepository)
                .Returns(_mockProductRepository.Object)
                .Verifiable();

            // আপনার হ্যান্ডলার GetByIdWithDetailsAsync কল করে
            _mockProductRepository.Setup(x => x.GetByIdWithDetailsAsync(product.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(product)
                .Verifiable();

            _mockProductRepository.Setup(x => x.RemoveAsync(product, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            _mockUnitOfWork.Setup(x => x.SaveAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            // Act
            await _handler.Handle(command, default);

            // Assert
            this.ShouldSatisfyAllConditions(
                () => _mockProductRepository.VerifyAll(),
                () => _mockUnitOfWork.VerifyAll()
            );
        }
    }
}