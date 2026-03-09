using SwiftCart.Application.Enums;
using SwiftCart.Domain.Entities;

namespace SwiftCart.Application.Interfaces;

public interface IAuthService
{
    User? CurrentUser { get; }
    RegistrationResult Register(string username, string password);
    User? Login(string username, string password);
    void Logout();
}
