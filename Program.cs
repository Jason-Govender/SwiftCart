using SwiftCart.Application.Services;
using SwiftCart.Infrastructure.Data;
using SwiftCart.Infrastructure.Persistence;
using SwiftCart.Presentation.Menus;

AppDb db = new AppDb();
JsonDataStore jsonDataStore = new JsonDataStore();
AuthService authService = new AuthService(db);
ProductService productService = new ProductService(db);
CustomerMenu customerMenu = new CustomerMenu(authService, productService);
AdministratorMenu administratorMenu = new AdministratorMenu(authService, productService);
MainMenu mainMenu = new MainMenu(authService, customerMenu, administratorMenu);

jsonDataStore.LoadUsers(db);
jsonDataStore.LoadProducts(db);
jsonDataStore.LoadCarts(db);
SeedData.SeedUsersIfEmpty(db);
SeedData.SeedProductsIfEmpty(db);

try
{
    mainMenu.Run();
}
finally
{
    jsonDataStore.SaveUsers(db);
    jsonDataStore.SaveProducts(db);
    jsonDataStore.SaveCarts(db);
}
