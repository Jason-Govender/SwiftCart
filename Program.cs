using SwiftCart.Application.Services;
using SwiftCart.Infrastructure.Data;
using SwiftCart.Infrastructure.Persistence;
using SwiftCart.Presentation.Menus;

AppDb db = new AppDb();
JsonDataStore jsonDataStore = new JsonDataStore();
AuthService authService = new AuthService(db);
ProductService productService = new ProductService(db);
CartService cartService = new CartService(db);
WalletService walletService = new WalletService(db);
OrderService orderService = new OrderService(db, cartService, walletService, productService);
ReviewService reviewService = new ReviewService(db, productService);
ReportService reportService = new ReportService(db);
CustomerMenu customerMenu = new CustomerMenu(authService, productService, cartService, walletService, orderService, reviewService);
AdministratorMenu administratorMenu = new AdministratorMenu(authService, productService, orderService, reviewService, reportService);
MainMenu mainMenu = new MainMenu(authService, customerMenu, administratorMenu);

jsonDataStore.LoadUsers(db);
jsonDataStore.LoadProducts(db);
jsonDataStore.LoadCarts(db);
jsonDataStore.LoadWallets(db);
jsonDataStore.LoadOrders(db);
jsonDataStore.LoadReviews(db);
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
    jsonDataStore.SaveWallets(db);
    jsonDataStore.SaveOrders(db);
    jsonDataStore.SaveReviews(db);
}
