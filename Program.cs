using SwiftCart.Application.Services;
using SwiftCart.Infrastructure.Data;
using SwiftCart.Infrastructure.Persistence;
using SwiftCart.Presentation.Menus;

AppDb db = new AppDb();
JsonDataStore jsonDataStore = new JsonDataStore();
AuthService authService = new AuthService(db);
MainMenu mainMenu = new MainMenu(authService);

jsonDataStore.LoadUsers(db);
SeedData.SeedUsersIfEmpty(db);

try
{
    mainMenu.Run();
}
finally
{
    jsonDataStore.SaveUsers(db);
}
