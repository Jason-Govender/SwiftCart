using SwiftCart.Application.Services;
using SwiftCart.Application.Interfaces;
using SwiftCart.Domain.Entities;
using SwiftCart.Domain.Enums;
using SwiftCart.Domain.OrderState;
using Moq;
using Xunit;

namespace SwiftCart.Tests.Application;

public class OrderServiceUpdateStatusTests
{
    private readonly Mock<IOrderRepository> _orderRepoMock;
    private readonly Mock<IPaymentRepository> _paymentRepoMock;
    private readonly Mock<ICartService> _cartServiceMock;
    private readonly Mock<IProductService> _productServiceMock;
    private readonly OrderStateMachine _stateMachine;
    private readonly OrderService _sut;

    public OrderServiceUpdateStatusTests()
    {
        _orderRepoMock = new Mock<IOrderRepository>();
        _paymentRepoMock = new Mock<IPaymentRepository>();
        _cartServiceMock = new Mock<ICartService>();
        _productServiceMock = new Mock<IProductService>();
        _stateMachine = new OrderStateMachine();
        _sut = new OrderService(
            _orderRepoMock.Object,
            _paymentRepoMock.Object,
            _cartServiceMock.Object,
            _productServiceMock.Object,
            _stateMachine);
    }

    [Fact]
    public void UpdateOrderStatus_OrderNotFound_ReturnsFalseWithMessage()
    {
        _orderRepoMock.Setup(r => r.GetById(999)).Returns((Order?)null);

        var (success, errorMessage) = _sut.UpdateOrderStatus(999, OrderStatus.Confirmed);

        Assert.False(success);
        Assert.Equal("Order not found.", errorMessage);
    }

    [Fact]
    public void UpdateOrderStatus_ValidTransition_UpdatesOrderAndReturnsTrue()
    {
        var order = new Order { Id = 1, CustomerId = 1, Status = OrderStatus.Pending, TotalAmount = 10, CreatedAt = DateTime.UtcNow, Items = new List<OrderItem>() };
        _orderRepoMock.Setup(r => r.GetById(1)).Returns(order);

        var (success, errorMessage) = _sut.UpdateOrderStatus(1, OrderStatus.Confirmed);

        Assert.True(success);
        Assert.Null(errorMessage);
        Assert.Equal(OrderStatus.Confirmed, order.Status);
    }

    [Fact]
    public void UpdateOrderStatus_InvalidTransition_ReturnsFalseWithAllowedTransitionsMessage()
    {
        var order = new Order { Id = 1, CustomerId = 1, Status = OrderStatus.Delivered, TotalAmount = 10, CreatedAt = DateTime.UtcNow, Items = new List<OrderItem>() };
        _orderRepoMock.Setup(r => r.GetById(1)).Returns(order);

        var (success, errorMessage) = _sut.UpdateOrderStatus(1, OrderStatus.Pending);

        Assert.False(success);
        Assert.NotNull(errorMessage);
        Assert.Contains("Delivered", errorMessage);
        Assert.Contains("Pending", errorMessage);
        Assert.Contains("terminal state", errorMessage);
        Assert.Equal(OrderStatus.Delivered, order.Status);
    }
}
