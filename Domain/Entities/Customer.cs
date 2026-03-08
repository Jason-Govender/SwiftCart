using SwiftCart.Domain.Enums;

namespace SwiftCart.Domain.Entities;

public class Customer : User
{
    public override UserRole Role => UserRole.Customer;
}
