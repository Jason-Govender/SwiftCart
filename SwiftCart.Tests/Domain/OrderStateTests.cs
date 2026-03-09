using SwiftCart.Domain.Enums;
using SwiftCart.Domain.OrderState;
using Xunit;

namespace SwiftCart.Tests.Domain;

public class OrderStateTests
{
    [Theory]
    [InlineData(OrderStatus.Pending)]
    [InlineData(OrderStatus.Confirmed)]
    [InlineData(OrderStatus.Shipped)]
    [InlineData(OrderStatus.Delivered)]
    [InlineData(OrderStatus.Cancelled)]
    public void GetState_ReturnsStateWithMatchingStatus(OrderStatus status)
    {
        var machine = new OrderStateMachine();
        var state = machine.GetState(status);
        Assert.Equal(status, state.Status);
    }

    [Fact]
    public void Pending_CanTransitionTo_ConfirmedAndCancelledOnly()
    {
        var machine = new OrderStateMachine();
        var state = machine.GetState(OrderStatus.Pending);

        Assert.True(state.CanTransitionTo(OrderStatus.Confirmed));
        Assert.True(state.CanTransitionTo(OrderStatus.Cancelled));
        Assert.False(state.CanTransitionTo(OrderStatus.Pending));
        Assert.False(state.CanTransitionTo(OrderStatus.Shipped));
        Assert.False(state.CanTransitionTo(OrderStatus.Delivered));

        var allowed = state.GetAllowedTransitions();
        Assert.Equal(2, allowed.Count);
        Assert.Contains(OrderStatus.Confirmed, allowed);
        Assert.Contains(OrderStatus.Cancelled, allowed);
    }

    [Fact]
    public void Confirmed_CanTransitionTo_ShippedAndCancelledOnly()
    {
        var machine = new OrderStateMachine();
        var state = machine.GetState(OrderStatus.Confirmed);

        Assert.True(state.CanTransitionTo(OrderStatus.Shipped));
        Assert.True(state.CanTransitionTo(OrderStatus.Cancelled));
        Assert.False(state.CanTransitionTo(OrderStatus.Pending));
        Assert.False(state.CanTransitionTo(OrderStatus.Confirmed));
        Assert.False(state.CanTransitionTo(OrderStatus.Delivered));

        var allowed = state.GetAllowedTransitions();
        Assert.Equal(2, allowed.Count);
        Assert.Contains(OrderStatus.Shipped, allowed);
        Assert.Contains(OrderStatus.Cancelled, allowed);
    }

    [Fact]
    public void Shipped_CanTransitionTo_DeliveredOnly()
    {
        var machine = new OrderStateMachine();
        var state = machine.GetState(OrderStatus.Shipped);

        Assert.True(state.CanTransitionTo(OrderStatus.Delivered));
        Assert.False(state.CanTransitionTo(OrderStatus.Pending));
        Assert.False(state.CanTransitionTo(OrderStatus.Confirmed));
        Assert.False(state.CanTransitionTo(OrderStatus.Shipped));
        Assert.False(state.CanTransitionTo(OrderStatus.Cancelled));

        var allowed = state.GetAllowedTransitions();
        Assert.Single(allowed);
        Assert.Equal(OrderStatus.Delivered, allowed[0]);
    }

    [Fact]
    public void Delivered_AllowsNoTransitions()
    {
        var machine = new OrderStateMachine();
        var state = machine.GetState(OrderStatus.Delivered);

        Assert.False(state.CanTransitionTo(OrderStatus.Pending));
        Assert.False(state.CanTransitionTo(OrderStatus.Confirmed));
        Assert.False(state.CanTransitionTo(OrderStatus.Shipped));
        Assert.False(state.CanTransitionTo(OrderStatus.Delivered));
        Assert.False(state.CanTransitionTo(OrderStatus.Cancelled));

        var allowed = state.GetAllowedTransitions();
        Assert.Empty(allowed);
    }

    [Fact]
    public void Cancelled_AllowsNoTransitions()
    {
        var machine = new OrderStateMachine();
        var state = machine.GetState(OrderStatus.Cancelled);

        Assert.False(state.CanTransitionTo(OrderStatus.Pending));
        Assert.False(state.CanTransitionTo(OrderStatus.Confirmed));
        Assert.False(state.CanTransitionTo(OrderStatus.Shipped));
        Assert.False(state.CanTransitionTo(OrderStatus.Delivered));
        Assert.False(state.CanTransitionTo(OrderStatus.Cancelled));

        var allowed = state.GetAllowedTransitions();
        Assert.Empty(allowed);
    }
}
