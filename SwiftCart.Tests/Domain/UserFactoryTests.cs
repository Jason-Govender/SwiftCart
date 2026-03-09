using SwiftCart.Domain.Entities;
using SwiftCart.Domain.Enums;
using SwiftCart.Domain.Factories;
using Xunit;

namespace SwiftCart.Tests.Domain;

public class UserFactoryTests
{
    [Fact]
    public void Create_Customer_ReturnsCustomerWithCorrectProperties()
    {
        var factory = new UserFactory();
        var user = factory.Create(UserRole.Customer, 1, "user1", "pass");

        var customer = Assert.IsType<Customer>(user);
        Assert.Equal(1, customer.Id);
        Assert.Equal("user1", customer.Username);
        Assert.Equal("pass", customer.Password);
    }

    [Fact]
    public void Create_Administrator_ReturnsAdministratorWithCorrectProperties()
    {
        var factory = new UserFactory();
        var user = factory.Create(UserRole.Administrator, 2, "admin", "secret");

        var admin = Assert.IsType<Administrator>(user);
        Assert.Equal(2, admin.Id);
        Assert.Equal("admin", admin.Username);
        Assert.Equal("secret", admin.Password);
    }

    [Fact]
    public void Create_InvalidRole_ThrowsArgumentOutOfRangeException()
    {
        var factory = new UserFactory();
        var invalidRole = (UserRole)99;

        var ex = Assert.Throws<ArgumentOutOfRangeException>(() =>
            factory.Create(invalidRole, 1, "u", "p"));

        Assert.Equal("role", ex.ParamName);
    }
}
