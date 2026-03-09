using SwiftCart.Domain.Entities;
using SwiftCart.Domain.Enums;
using SwiftCart.Domain.Factories;

namespace SwiftCart.Infrastructure.Data;

public static class SeedData
{
    public static void SeedUsersIfEmpty(AppDb db, IUserFactory userFactory)
    {
        if (db.Users.Count > 0)
            return;

        db.Users.Add(userFactory.Create(UserRole.Administrator, 1, "admin", "admin"));
    }

    public static void SeedProductsIfEmpty(AppDb db)
    {
        if (db.Products.Count > 0)
            return;

        db.Products.AddRange(new[]
        {
            new Product { Id = 1, Name = "Widget A", Description = "A useful widget", Price = 9.99m, StockQuantity = 50 },
            new Product { Id = 2, Name = "Gadget B", Description = "Handy gadget for daily use", Price = 19.99m, StockQuantity = 25 },
            new Product { Id = 3, Name = "Tool Kit", Description = "Essential tools for repairs", Price = 34.99m, StockQuantity = 10 }
        });
    }
}
