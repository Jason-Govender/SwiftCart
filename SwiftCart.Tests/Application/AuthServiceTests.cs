using SwiftCart.Application.Enums;
using SwiftCart.Application.Services;
using SwiftCart.Domain.Entities;
using SwiftCart.Domain.Factories;
using SwiftCart.Application.Interfaces;
using Moq;
using Xunit;

namespace SwiftCart.Tests.Application;

public class AuthServiceTests
{
    private readonly Mock<IUserRepository> _userRepoMock;
    private readonly IUserFactory _userFactory;
    private readonly AuthService _sut;

    public AuthServiceTests()
    {
        _userRepoMock = new Mock<IUserRepository>();
        _userFactory = new UserFactory();
        _sut = new AuthService(_userRepoMock.Object, _userFactory);
    }

    [Theory]
    [InlineData("", "ValidPass1!")]
    [InlineData("   ", "ValidPass1!")]
    [InlineData("user", "")]
    [InlineData("user", "   ")]
    public void Register_EmptyUsernameOrPassword_ReturnsEmptyCredentials(string username, string password)
    {
        var result = _sut.Register(username, password);
        Assert.Equal(RegistrationResult.EmptyCredentials, result);
        _userRepoMock.Verify(r => r.Add(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public void Register_NullUsername_ReturnsEmptyCredentials()
    {
        var result = _sut.Register(null!, "ValidPass1!");
        Assert.Equal(RegistrationResult.EmptyCredentials, result);
    }

    [Fact]
    public void Register_NullPassword_ReturnsEmptyCredentials()
    {
        var result = _sut.Register("user", null!);
        Assert.Equal(RegistrationResult.EmptyCredentials, result);
    }

    [Theory]
    [InlineData("short")]
    [InlineData("NoDigit!")]
    [InlineData("NoSymbol1")]
    [InlineData("nouppercase1!")]
    [InlineData("NOLOWERCASE1!")]
    public void Register_WeakPassword_ReturnsWeakPassword(string password)
    {
        _userRepoMock.Setup(r => r.ExistsWithUsername(It.IsAny<string>())).Returns(false);

        var result = _sut.Register("user", password);

        Assert.Equal(RegistrationResult.WeakPassword, result);
        _userRepoMock.Verify(r => r.Add(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public void Register_DuplicateUsername_ReturnsDuplicateUsername()
    {
        _userRepoMock.Setup(r => r.ExistsWithUsername("existing")).Returns(true);

        var result = _sut.Register("existing", "ValidPass1!");

        Assert.Equal(RegistrationResult.DuplicateUsername, result);
        _userRepoMock.Verify(r => r.Add(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public void Register_ValidInput_AddsCustomerAndReturnsSuccess()
    {
        _userRepoMock.Setup(r => r.ExistsWithUsername("newuser")).Returns(false);
        _userRepoMock.Setup(r => r.GetNextId()).Returns(1);
        User? capturedUser = null;
        _userRepoMock.Setup(r => r.Add(It.IsAny<User>())).Callback<User>(u => capturedUser = u);

        var result = _sut.Register("newuser", "ValidPass1!");

        Assert.Equal(RegistrationResult.Success, result);
        _userRepoMock.Verify(r => r.Add(It.IsAny<User>()), Times.Once);
        Assert.NotNull(capturedUser);
        Assert.IsType<Customer>(capturedUser);
        Assert.Equal(1, capturedUser.Id);
        Assert.Equal("newuser", capturedUser.Username);
    }

    [Theory]
    [InlineData("", "pass")]
    [InlineData("   ", "pass")]
    [InlineData("user", "")]
    [InlineData("user", "   ")]
    public void Login_EmptyUsernameOrPassword_ReturnsNullAndClearsCurrentUser(string username, string password)
    {
        _sut.Login("someone", "pass");
        var result = _sut.Login(username, password);

        Assert.Null(result);
        Assert.Null(_sut.CurrentUser);
    }

    [Fact]
    public void Login_NullUsername_ReturnsNull()
    {
        var result = _sut.Login(null!, "pass");
        Assert.Null(result);
    }

    [Fact]
    public void Login_NullPassword_ReturnsNull()
    {
        var result = _sut.Login("user", null!);
        Assert.Null(result);
    }

    [Fact]
    public void Login_ValidCredentials_ReturnsUserAndSetsCurrentUser()
    {
        var customer = new Customer { Id = 1, Username = "joe", Password = "Pass1!" };
        _userRepoMock.Setup(r => r.FindByCredentials("joe", "Pass1!")).Returns(customer);

        var result = _sut.Login("joe", "Pass1!");

        Assert.Same(customer, result);
        Assert.Same(customer, _sut.CurrentUser);
    }
}
