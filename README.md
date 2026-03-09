# SwiftCart

A console-based e-commerce backend system built in C# (.NET 10). SwiftCart simulates the core backend operations of an online shopping platform, supporting two user roles — Customer and Administrator — through a structured, menu-driven interface.

## Features

### Customer
- Register and log in with password strength enforcement
- Browse and search the product catalog with ratings
- Manage a shopping cart (add, update quantity, remove items)
- Checkout with payment method choice (Wallet or Cash on Delivery)
- Top up wallet funds
- View order history and track order status
- View notifications (order placed and order status updates)
- Submit and update product reviews (1–5 star rating with comment)

### Administrator
- Add, update, and delete products
- Restock inventory
- View all orders and update their status
- Monitor low stock products with a configurable threshold
- Generate sales reports — summary, top products by revenue, and revenue by day/week/month

## Architecture

The project follows a clean layered architecture:

```
SwiftCart/
├── Domain/
│   ├── Entities/        # User, Customer, Administrator, Product, Cart,
│   │                    # CartItem, Order, OrderItem, Payment, Review, Wallet, Notification
│   ├── Enums/           # UserRole, OrderStatus
│   ├── Factories/       # IUserFactory, UserFactory (user creation by role)
│   └── OrderState/      # IOrderState, OrderStateMachine, state classes (Pending, Confirmed, Shipped, Delivered, Cancelled)
├── Application/
│   ├── Interfaces/     # Service + repository + strategy + observer interfaces
│   ├── Services/        # Concrete service implementations
│   ├── PaymentStrategies/  # WalletPaymentStrategy, CashOnDeliveryPaymentStrategy
│   ├── Observers/       # OrderNotificationObserver (order events → notifications)
│   ├── Helpers/         # InputHelper (validated console input)
│   └── Enums/           # RegistrationResult
├── Infrastructure/
│   ├── Data/            # AppDb (in-memory collections), SeedData
│   ├── Repositories/    # User, Product, Cart, Order, Payment, Wallet, Review repositories
│   └── Persistence/     # JsonDataStore (JSON file read/write)
├── Presentation/
│   └── Menus/           # MainMenu, CustomerMenu, AdministratorMenu
└── Common/
    └── Constants/       # AppConstants
```

### Design Patterns (Submission 2)

- **Factory** — `IUserFactory` / `UserFactory` in `Domain/Factories/`. User creation (Customer vs Administrator) is centralised here so `AuthService` and `SeedData` do not depend on concrete user types; adding a new role only requires changing the factory.
- **Observer** — `IOrderObserver` and `OrderNotificationObserver` in `Application/`. When an order is placed or its status changes, `OrderService` notifies all subscribed observers without knowing what they do. The notification observer writes to `AppDb.Notifications`; customers see these via **View Notifications**. New reactions (e.g. audit log, email) can be added by implementing the interface and subscribing — no change to `OrderService`.
- **Strategy** — `IPaymentStrategy` and implementations (`WalletPaymentStrategy`, `CashOnDeliveryPaymentStrategy`) in `Application/PaymentStrategies/`. Checkout delegates payment to the selected strategy (`Pay` / `Refund`). `OrderService` has no wallet-specific logic; new payment methods are added by implementing the interface and choosing the strategy at checkout (Open/Closed Principle).
- **Repository** — Seven repository interfaces in `Application/Interfaces/` and implementations in `Infrastructure/Repositories/` (User, Product, Cart, Order, Payment, Wallet, Review). Services depend only on repository interfaces, not on `AppDb`. Data access and ID generation live in one place; swapping storage would require changing only the repository layer.
- **State** — `IOrderState` and `OrderStateMachine` in `Domain/OrderState/`, with concrete states for each `OrderStatus` (Pending, Confirmed, Shipped, Delivered, Cancelled). When an administrator updates an order’s status, `OrderService` uses the state machine to allow only valid transitions (`CanTransitionTo`). Invalid transitions are rejected with a clear message; adding or changing allowed transitions is done by adjusting the state classes.

### Key Design Decisions

- **Interfaces** — All services (and repositories, payment strategies, order observers) are defined by interfaces and consumed through them, supporting extensibility and clean separation of concerns.
- **Polymorphism** — `User` is an abstract base class; `Customer` and `Administrator` extend it with role-specific behaviour.
- **LINQ** — Used in repository implementations and report logic for querying, filtering, grouping, and aggregating collections.
- **Exception handling** — All menu handler methods and the critical `OrderService.PlaceOrder` are wrapped in `try/catch` blocks. Services signal expected failures through typed return values (`bool`, `null`, or result tuples) rather than exceptions.
- **Persistence** — All data is stored as JSON files alongside the executable. Data is saved after every completed user action and again on application shutdown via a `finally` block as a safety net.

## Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)

### Run

```bash
dotnet run
```

### Default Credentials

On first run, a default administrator account is seeded automatically:

| Username | Password |
|----------|----------|
| `admin`  | `admin`  |


### Password Requirements

Customer registration enforces the following password policy:
- Minimum 8 characters
- At least one uppercase letter
- At least one lowercase letter
- At least one digit
- At least one symbol

## Data Storage

All data is persisted to JSON files in the application output directory:

| File | Contents |
|------|----------|
| `users.json` | Registered users (customers and administrators) |
| `products.json` | Product catalog |
| `carts.json` | Active shopping carts |
| `wallets.json` | Customer wallet balances |
| `orders.json` | Placed orders and their items |
| `reviews.json` | Product reviews |
| `payments.json` | Payment records linked to orders |
| `notifications.json` | Order notifications (placed and status updates) |

## Dependencies

| Package | Version | Purpose |
|---------|---------|---------|
| [Newtonsoft.Json](https://www.newtonsoft.com/json) | 13.0.3 | JSON serialisation with polymorphic type support |

## Testing

Unit tests live in the `SwiftCart.Tests` project (xUnit + Moq). Run all tests from the repository root:

```bash
dotnet test
```

Or target the test project explicitly:

```bash
dotnet test --project SwiftCart.Tests
```

Tests cover: order state machine and valid/invalid status transitions, `UserFactory` (Customer/Administrator creation and invalid role), `AuthService` (registration validation and login with mocked `IUserRepository`), and `OrderService.UpdateOrderStatus` (order not found, valid transition, invalid transition with terminal state message).

## Menu Structure

```
Main Menu
├── Register
├── Login
│   ├── Customer Menu
│   │   ├── Browse Products
│   │   ├── Search Products
│   │   ├── Add Product to Cart
│   │   ├── View Cart
│   │   ├── Update Cart
│   │   ├── Checkout (Wallet or Cash on Delivery)
│   │   ├── View Wallet Balance
│   │   ├── Add Wallet Funds
│   │   ├── View Order History
│   │   ├── Track Orders
│   │   ├── Review Products
│   │   ├── View Notifications
│   │   └── Logout
│   └── Administrator Menu
│       ├── Add Product
│       ├── Update Product
│       ├── Delete Product
│       ├── Restock Product
│       ├── View Products
│       ├── View Orders
│       ├── Update Order Status
│       ├── View Low Stock Products
│       ├── Generate Sales Reports
│       │   ├── Sales Summary
│       │   ├── Top Products
│       │   └── Revenue by Period
│       └── Logout
└── Exit
```
