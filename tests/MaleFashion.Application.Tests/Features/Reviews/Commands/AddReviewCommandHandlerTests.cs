using Autofac.Extras.Moq;
using MaleFashion.Application.Contracts;
using MaleFashion.Application.Contracts.Repositories;
using MaleFashion.Application.Exceptions;
using MaleFashion.Application.Features.Reviews.Command;
using MaleFashion.Domain.Entites;
using Moq;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaleFashion.Application.Tests.Features.Reviews.Commands
{
    [TestFixture]
    public class AddReviewCommandHandlerTests
    {
        private AutoMock _mock;
        private AddReviewCommandHandler _handler;
        private Mock<IApplicationUnitOfwork> _mockUnitOfWork;
        private Mock<IProductRepository> _mockProductRepository;
        private Mock<IReviewRepository> _mockReviewRepository;

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
            _mockReviewRepository = _mock.Mock<IReviewRepository>();
            _handler = _mock.Create<AddReviewCommandHandler>();
        }

        [TearDown]
        public void TearDown()
        {
            _mockProductRepository?.Reset();
            _mockReviewRepository?.Reset();
            _mockUnitOfWork?.Reset();
        }

        [Test]
        public async Task Handle_ProductNotFound_ThrowsNotFoundException()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var customerId = Guid.NewGuid();
            var command = new AddReviewCommand(productId, customerId, 5, "Great quality!");

            _mockUnitOfWork.SetupGet(x => x.ProductRepository)
                .Returns(_mockProductRepository.Object)
                .Verifiable();

            _mockProductRepository.Setup(x => x.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
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
                () => _mockReviewRepository.Verify(x => x.AddAsync(It.IsAny<Review>(), It.IsAny<CancellationToken>()), Times.Never),
                () => _mockUnitOfWork.Verify(x => x.SaveAsync(It.IsAny<CancellationToken>()), Times.Never)
            );
        }

        [Test]
        public async Task Handle_CustomerAlreadyReviewed_ThrowsDuplicateDataException()
        {
            // Arrange
            var product = CreateSampleProduct();
            var customerId = Guid.NewGuid();
            var command = new AddReviewCommand(product.Id, customerId, 4, "Second attempt review");

            _mockUnitOfWork.SetupGet(x => x.ProductRepository)
                .Returns(_mockProductRepository.Object)
                .Verifiable();

            _mockProductRepository.Setup(x => x.GetByIdAsync(product.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(product)
                .Verifiable();

            _mockUnitOfWork.SetupGet(x => x.ReviewRepository)
                .Returns(_mockReviewRepository.Object)
                .Verifiable();

            _mockReviewRepository.Setup(x => x.HasCustomerReviewedProductAsync(product.Id, customerId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true)
                .Verifiable();

            // Act & Assert
            var ex = await Should.ThrowAsync<DuplicateDataException>(async () =>
            {
                await _handler.Handle(command, default);
            });

            this.ShouldSatisfyAllConditions(
                () => ex.Message.ShouldContain("already submitted a review"),
                () => _mockProductRepository.VerifyAll(),
                () => _mockReviewRepository.VerifyAll(),
                () => _mockReviewRepository.Verify(x => x.AddAsync(It.IsAny<Review>(), It.IsAny<CancellationToken>()), Times.Never),
                () => _mockUnitOfWork.Verify(x => x.SaveAsync(It.IsAny<CancellationToken>()), Times.Never)
            );
        }

        [Test]
        public async Task Handle_ValidReview_AddsReviewAndSetsCommandId()
        {
            // Arrange
            var product = CreateSampleProduct();
            var customerId = Guid.NewGuid();
            var command = new AddReviewCommand(product.Id, customerId, 5, "Outstanding fit and fabric!");

            _mockUnitOfWork.SetupGet(x => x.ProductRepository)
                .Returns(_mockProductRepository.Object)
                .Verifiable();

            _mockProductRepository.Setup(x => x.GetByIdAsync(product.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(product)
                .Verifiable();

            _mockUnitOfWork.SetupGet(x => x.ReviewRepository)
                .Returns(_mockReviewRepository.Object)
                .Verifiable();

            _mockReviewRepository.Setup(x => x.HasCustomerReviewedProductAsync(product.Id, customerId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false)
                .Verifiable();

            _mockReviewRepository.Setup(x => x.AddAsync(It.IsAny<Review>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            _mockUnitOfWork.Setup(x => x.SaveAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            // Act
            await _handler.Handle(command, default);

            // Assert
            this.ShouldSatisfyAllConditions(
                () => command.Id.ShouldNotBe(Guid.Empty),
                () => _mockProductRepository.VerifyAll(),
                () => _mockReviewRepository.VerifyAll(),
                () => _mockUnitOfWork.VerifyAll()
            );
        }

        private static Product CreateSampleProduct()
        {
            return Product.Create(
                name: "Oxford Cotton Shirt",
                slug: "oxford-cotton-shirt",
                description: "Classic slim formal and casual shirt",
                price: 45.00m,
                discountPrice: null,
                brand: "MaleFashion",
                imageName: "shirt_oxford.jpg",
                categoryId: Guid.NewGuid(),
                createdAtUtc: DateTime.UtcNow,
                isActive: true
            );
        }
    }
}
