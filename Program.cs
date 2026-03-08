using SwiftCart.Application.Services;
using SwiftCart.Infrastructure.Data;
using SwiftCart.Infrastructure.Persistence;
using SwiftCart.Presentation.Menus;

AppDb db = new AppDb();
JsonDataStore jsonDataStore = new JsonDataStore();
AuthService authService = new AuthService(db);
CustomerMenu customerMenu = new CustomerMenu(authService);
AdministratorMenu administratorMenu = new AdministratorMenu(authService);
MainMenu mainMenu = new MainMenu(authService, customerMenu, administratorMenu);

jsonDataStore.LoadUsers(db);
jsonDataStore.LoadProducts(db);
SeedData.SeedUsersIfEmpty(db);

try
{
    mainMenu.Run();
}
finally
{
    jsonDataStore.SaveUsers(db);
    jsonDataStore.SaveProducts(db);
}
