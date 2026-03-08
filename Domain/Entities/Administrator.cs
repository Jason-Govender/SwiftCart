using SwiftCart.Domain.Enums;

namespace SwiftCart.Domain.Entities;

public class Administrator : User
{
    public override UserRole Role => UserRole.Administrator;
}
