# SwiftCart

A console-based e-commerce backend system built in C# (.NET 10). SwiftCart simulates the core backend operations of an online shopping platform, supporting two user roles — Customer and Administrator — through a structured, menu-driven interface.

## Features

### Customer
- Register and log in with password strength enforcement
- Browse and search the product catalog with ratings
- Manage a shopping cart (add, update quantity, remove items)
- Checkout and pay via wallet balance
- Top up wallet funds
- View order history and track order status
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
│   │                    # CartItem, Order, OrderItem, Payment, Review, Wallet
│   └── Enums/           # UserRole, OrderStatus
├── Application/
│   ├── Interfaces/      # IAuthService, IProductService, ICartService,
│   │                    # IOrderService, IWalletService, IReviewService, IReportService
│   ├── Services/        # Concrete service implementations
│   ├── Helpers/         # InputHelper (validated console input)
│   └── Enums/           # RegistrationResult
├── Infrastructure/
│   ├── Data/            # AppDb (in-memory collections), SeedData
│   └── Persistence/     # JsonDataStore (JSON file read/write)
├── Presentation/
│   └── Menus/           # MainMenu, CustomerMenu, AdministratorMenu
└── Common/
    └── Constants/       # AppConstants
```

### Key Design Decisions

- **Interfaces** — All services are defined by interfaces (`IXxxService`) and consumed through them across menus and cross-service dependencies, supporting extensibility and clean separation of concerns.
- **Polymorphism** — `User` is an abstract base class; `Customer` and `Administrator` extend it with role-specific behaviour.
- **LINQ** — Used throughout for querying, filtering, grouping, and aggregating in-memory collections.
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

## Dependencies

| Package | Version | Purpose |
|---------|---------|---------|
| [Newtonsoft.Json](https://www.newtonsoft.com/json) | 13.0.3 | JSON serialisation with polymorphic type support |

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
│   │   ├── Checkout
│   │   ├── View Wallet Balance
│   │   ├── Add Wallet Funds
│   │   ├── View Order History
│   │   ├── Track Orders
│   │   ├── Review Products
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
