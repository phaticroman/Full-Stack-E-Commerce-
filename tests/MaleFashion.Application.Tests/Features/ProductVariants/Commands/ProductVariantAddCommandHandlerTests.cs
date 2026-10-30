using System;
using System.Threading;
using System.Threading.Tasks;
using Autofac.Extras.Moq;
using MaleFashion.Application.Contracts;
using MaleFashion.Application.Contracts.Repositories;
using MaleFashion.Application.Exceptions;
using MaleFashion.Application.Features.ProductVariants.Command;
using MaleFashion.Domain.Entites;
using Moq;
using NUnit.Framework;
using Shouldly;

namespace MaleFashion.Application.Tests.Features.ProductVariants.Commands
{
    [TestFixture]
    public class ProductVariantAddCommandHandlerTests
    {
        private AutoMock _mock;
        private ProductVariantAddCommandHandler _handler;
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
            _handler = _mock.Create<ProductVariantAddCommandHandler>();
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
            var command = new ProductVariantAddCommand
            {
                ProductId = Guid.NewGuid(),
                SKU = "MF-TSHIRT-BLK-M",
                Size = "M",
                Color = "Black",
                StockQuantity = 15
            };

            _mockUnitOfWork.SetupGet(x => x.ProductRepository)
                .Returns(_mockProductRepository.Object)
                .Verifiable();

            _mockProductRepository.Setup(x => x.GetByIdWithDetailsAsync(command.ProductId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Product?)null)
                .Verifiable();

            // Act & Assert
            var ex = await Should.ThrowAsync<NotFoundException>(async () =>
            {
                await _handler.Handle(command, default);
            });

            this.ShouldSatisfyAllConditions(
                () => ex.Message.ShouldContain(command.ProductId.ToString()),
                () => _mockProductRepository.VerifyAll(),
                () => _mockUnitOfWork.Verify(x => x.SaveAsync(It.IsAny<CancellationToken>()), Times.Never)
            );
        }

        [Test]
        public async Task Handle_DuplicateSku_ThrowsDuplicateDataException()
        {
            // Arrange
            var product = CreateSampleProduct();

            var command = new ProductVariantAddCommand
            {
                ProductId = product.Id,
                SKU = "DUPLICATE-SKU-001",
                Size = "L",
                Color = "White",
                StockQuantity = 10
            };

            _mockUnitOfWork.SetupGet(x => x.ProductRepository)
                .Returns(_mockProductRepository.Object)
                .Verifiable();

            _mockProductRepository.Setup(x => x.GetByIdWithDetailsAsync(product.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(product)
                .Verifiable();

            _mockProductRepository.Setup(x => x.IsDuplicateSkuAsync(command.SKU, null, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true)
                .Verifiable();

            // Act & Assert
            var ex = await Should.ThrowAsync<DuplicateDataException>(async () =>
            {
                await _handler.Handle(command, default);
            });

            this.ShouldSatisfyAllConditions(
                () => ex.Message.ShouldContain(command.SKU),
                () => _mockProductRepository.VerifyAll(),
                () => _mockUnitOfWork.Verify(x => x.SaveAsync(It.IsAny<CancellationToken>()), Times.Never)
            );
        }

        [Test]
        public async Task Handle_UniqueSku_AddsVariantAndSaves()
        {
            // Arrange
            var product = CreateSampleProduct();

            var command = new ProductVariantAddCommand
            {
                ProductId = product.Id,
                SKU = "UNIQUE-SKU-999",
                Size = "XL",
                Color = "Navy",
                StockQuantity = 20
            };

            _mockUnitOfWork.SetupGet(x => x.ProductRepository)
                .Returns(_mockProductRepository.Object)
                .Verifiable();

            _mockProductRepository.Setup(x => x.GetByIdWithDetailsAsync(product.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(product)
                .Verifiable();

            _mockProductRepository.Setup(x => x.IsDuplicateSkuAsync(command.SKU, null, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false)
                .Verifiable();

            _mockUnitOfWork.Setup(x => x.SaveAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            // Act
            var variantId = await _handler.Handle(command, default);

            // Assert
            this.ShouldSatisfyAllConditions(
                () => variantId.ShouldNotBe(Guid.Empty),
                () => product.Variants.Count.ShouldBe(1),
                () => _mockProductRepository.VerifyAll(),
                () => _mockUnitOfWork.VerifyAll()
            );
        }

        private static Product CreateSampleProduct()
        {
            return Product.Create(
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
        }
    }
}