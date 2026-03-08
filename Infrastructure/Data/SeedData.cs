using SwiftCart.Domain.Entities;

namespace SwiftCart.Infrastructure.Data;

public static class SeedData
{
    public static void SeedUsersIfEmpty(AppDb db)
    {
        if (db.Users.Count > 0)
            return;

        db.Users.Add(new Administrator
        {
            Id = 1,
            Username = "admin",
            Password = "admin"
        });
    }
}
