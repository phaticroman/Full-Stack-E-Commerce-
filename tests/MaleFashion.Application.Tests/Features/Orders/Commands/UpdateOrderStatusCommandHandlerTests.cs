using System;
using System.Threading;
using System.Threading.Tasks;
using Autofac.Extras.Moq;
using MaleFashion.Application.Contracts;
using MaleFashion.Application.Contracts.Repositories;
using MaleFashion.Application.Exceptions;
using MaleFashion.Application.Features.Orders.Command;
using MaleFashion.Domain.Entites;
using MaleFashion.Domain.Enums;
using Moq;
using NUnit.Framework;
using Shouldly;
using MaleFashion.Domain.Exceptions;
using DomainException = MaleFashion.Domain.Exceptions.DomainException;

namespace MaleFashion.Application.Tests.Features.Orders.Commands
{
    [TestFixture]
    public class UpdateOrderStatusCommandHandlerTests
    {
        private AutoMock _mock;
        private UpdateOrderStatusCommandHandler _handler;
        private Mock<IApplicationUnitOfwork> _mockUnitOfWork;
        private Mock<IOrderRepository> _mockOrderRepository;

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
            _mockOrderRepository = _mock.Mock<IOrderRepository>();
            _handler = _mock.Create<UpdateOrderStatusCommandHandler>();
        }

        [TearDown]
        public void TearDown()
        {
            _mockOrderRepository?.Reset();
            _mockUnitOfWork?.Reset();
        }

        [Test]
        public async Task Handle_OrderNotFound_ThrowsNotFoundException()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var command = new UpdateOrderStatusCommand(orderId, OrderStatus.Confirmed);

            _mockUnitOfWork.SetupGet(x => x.OrderRepository)
                .Returns(_mockOrderRepository.Object)
                .Verifiable();

            _mockOrderRepository.Setup(x => x.GetByIdAsync(orderId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Order?)null)
                .Verifiable();

            // Act & Assert
            var ex = await Should.ThrowAsync<NotFoundException>(async () =>
            {
                await _handler.Handle(command, default);
            });

            this.ShouldSatisfyAllConditions(
                () => ex.Message.ShouldContain(orderId.ToString()),
                () => _mockOrderRepository.VerifyAll(),
                () => _mockUnitOfWork.Verify(x => x.SaveAsync(It.IsAny<CancellationToken>()), Times.Never)
            );
        }

        [Test]
        public async Task Handle_SameStatusProvided_ReturnsTrueWithoutSaving()
        {

            var order = CreateSampleOrder();
            var command = new UpdateOrderStatusCommand(order.Id, OrderStatus.Pending);

            _mockUnitOfWork.SetupGet(x => x.OrderRepository)
                .Returns(_mockOrderRepository.Object)
                .Verifiable();

            _mockOrderRepository.Setup(x => x.GetByIdAsync(order.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(order)
                .Verifiable();

            // Act
            var result = await _handler.Handle(command, default);

            // Assert
            this.ShouldSatisfyAllConditions(
                () => result.ShouldBeTrue(),
                () => order.Status.ShouldBe(OrderStatus.Pending),
                () => _mockOrderRepository.VerifyAll(),
                () => _mockUnitOfWork.Verify(x => x.SaveAsync(It.IsAny<CancellationToken>()), Times.Never)
            );
        }

        [Test]
        public async Task Handle_ValidStatusChangeToConfirmed_UpdatesStatusAndSaves()
        {
            // Arrange
            var order = CreateSampleOrder();
            var command = new UpdateOrderStatusCommand(order.Id, OrderStatus.Confirmed);

            _mockUnitOfWork.SetupGet(x => x.OrderRepository)
                .Returns(_mockOrderRepository.Object)
                .Verifiable();

            _mockOrderRepository.Setup(x => x.GetByIdAsync(order.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(order)
                .Verifiable();

            _mockUnitOfWork.Setup(x => x.SaveAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            // Act
            var result = await _handler.Handle(command, default);

            // Assert
            this.ShouldSatisfyAllConditions(
                () => result.ShouldBeTrue(),
                () => order.Status.ShouldBe(OrderStatus.Confirmed),
                () => _mockOrderRepository.VerifyAll(),
                () => _mockUnitOfWork.VerifyAll()
            );
        }

        [Test]
public async Task Handle_InvalidTransition_ThrowsDomainException()
{
    // Arrange
    var order = CreateSampleOrder();
    var command = new UpdateOrderStatusCommand(order.Id, OrderStatus.Delivered);

    _mockUnitOfWork.SetupGet(x => x.OrderRepository)
        .Returns(_mockOrderRepository.Object)
        .Verifiable();

    _mockOrderRepository.Setup(x => x.GetByIdAsync(order.Id, It.IsAny<CancellationToken>()))
        .ReturnsAsync(order)
        .Verifiable();

    // Act & Assert (স্পষ্টভাবে Domain নেমস্পেসের এক্সেপশনটি উল্লেখ করা হয়েছে)
    var ex = await Should.ThrowAsync<DomainException>(async () =>
    {
        await _handler.Handle(command, default);
    });

    this.ShouldSatisfyAllConditions(
        () => ex.ShouldNotBeNull(),
        () => order.Status.ShouldBe(OrderStatus.Pending),
        () => _mockOrderRepository.VerifyAll(),
        () => _mockUnitOfWork.Verify(x => x.SaveAsync(It.IsAny<CancellationToken>()), Times.Never)
    );
}

        private static Order CreateSampleOrder()
        {
            return Order.Create(
                customerId: Guid.NewGuid(),
                shippingFullName: "Roman Hassan",
                shippingPhone: "01700000000",
                shippingEmail: "roman@example.com",
                shippingCity: "Dhaka",
                shippingPostalCode: "1200"
            );
        }
    }
}