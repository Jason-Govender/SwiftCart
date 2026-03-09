using SwiftCart.Application.Observers;
using SwiftCart.Application.Services;
using SwiftCart.Domain.Factories;
using SwiftCart.Domain.OrderState;
using SwiftCart.Infrastructure.Data;
using SwiftCart.Infrastructure.Persistence;
using SwiftCart.Infrastructure.Repositories;
using SwiftCart.Presentation.Menus;

AppDb db = new AppDb();
JsonDataStore jsonDataStore = new JsonDataStore();
UserFactory userFactory = new UserFactory();

UserRepository userRepo = new UserRepository(db);
ProductRepository productRepo = new ProductRepository(db);
CartRepository cartRepo = new CartRepository(db);
OrderRepository orderRepo = new OrderRepository(db);
PaymentRepository paymentRepo = new PaymentRepository(db);
WalletRepository walletRepo = new WalletRepository(db);
ReviewRepository reviewRepo = new ReviewRepository(db);

AuthService authService = new AuthService(userRepo, userFactory);
ProductService productService = new ProductService(productRepo);
CartService cartService = new CartService(cartRepo, productRepo);
WalletService walletService = new WalletService(walletRepo);
OrderStateMachine orderStateMachine = new OrderStateMachine();
OrderService orderService = new OrderService(orderRepo, paymentRepo, cartService, productService, orderStateMachine);
OrderNotificationObserver notificationObserver = new OrderNotificationObserver(db);
orderService.Subscribe(notificationObserver);
ReviewService reviewService = new ReviewService(reviewRepo, productService);
ReportService reportService = new ReportService(orderRepo, productRepo);

Action saveAll = () =>
{
    jsonDataStore.SaveUsers(db);
    jsonDataStore.SaveProducts(db);
    jsonDataStore.SaveCarts(db);
    jsonDataStore.SaveWallets(db);
    jsonDataStore.SaveOrders(db);
    jsonDataStore.SaveReviews(db);
    jsonDataStore.SavePayments(db);
    jsonDataStore.SaveNotifications(db);
};
CustomerMenu customerMenu = new CustomerMenu(authService, productService, cartService, walletService, orderService, reviewService, db, saveAll);
AdministratorMenu administratorMenu = new AdministratorMenu(authService, productService, orderService, reviewService, reportService, saveAll);
MainMenu mainMenu = new MainMenu(authService, customerMenu, administratorMenu, saveAll);

jsonDataStore.LoadUsers(db);
jsonDataStore.LoadProducts(db);
jsonDataStore.LoadCarts(db);
jsonDataStore.LoadWallets(db);
jsonDataStore.LoadOrders(db);
jsonDataStore.LoadReviews(db);
jsonDataStore.LoadPayments(db);
jsonDataStore.LoadNotifications(db);
SeedData.SeedUsersIfEmpty(db, userFactory);
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
    jsonDataStore.SavePayments(db);
    jsonDataStore.SaveNotifications(db);
}
