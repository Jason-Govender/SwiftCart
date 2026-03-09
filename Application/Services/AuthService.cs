using SwiftCart.Application.Enums;
using SwiftCart.Application.Interfaces;
using SwiftCart.Domain.Entities;
using SwiftCart.Domain.Enums;
using SwiftCart.Domain.Factories;

namespace SwiftCart.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepo;
    private readonly IUserFactory _userFactory;

    public User? CurrentUser { get; private set; }

    public AuthService(IUserRepository userRepo, IUserFactory userFactory)
    {
        _userRepo = userRepo;
        _userFactory = userFactory;
    }

    public RegistrationResult Register(string username, string password)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            return RegistrationResult.EmptyCredentials;

        if (!MeetsPasswordStrength(password))
            return RegistrationResult.WeakPassword;

        if (_userRepo.ExistsWithUsername(username.Trim()))
            return RegistrationResult.DuplicateUsername;

        int nextId = _userRepo.GetNextId();
        User customer = _userFactory.Create(UserRole.Customer, nextId, username.Trim(), password);
        _userRepo.Add(customer);
        return RegistrationResult.Success;
    }

    public User? Login(string username, string password)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            CurrentUser = null;
            return null;
        }

        User? user = _userRepo.FindByCredentials(username.Trim(), password);
        CurrentUser = user;
        return user;
    }

    public void Logout()
    {
        CurrentUser = null;
    }

    private static bool MeetsPasswordStrength(string password)
    {
        if (password.Length < 8) return false;
        if (!password.Any(char.IsUpper)) return false;
        if (!password.Any(char.IsLower)) return false;
        if (!password.Any(char.IsDigit)) return false;
        if (!password.Any(c => !char.IsLetterOrDigit(c))) return false;
        return true;
    }
}
