# Architecture

Always read `spec.md` before implementing features. This document defines how the architecture should be applied to this specific project.

## Architectural Style

This project uses lightweight Domain-Driven Design for a C# console application.

The goal is clean separation of responsibilities without enterprise overengineering.

## Layers

### Domain

Contains core business entities and enums.

Examples:

- User
- Customer
- Administrator
- Product
- Cart
- CartItem
- Order
- OrderItem
- Payment
- Review
- UserRole
- OrderStatus

Rules:

- No console input/output here.
- No persistence logic here.
- Domain entities should model business meaning and relationships.

### Application

Contains use-case logic and orchestration.

Examples:

- AuthService
- ProductService
- CartService
- OrderService
- WalletService
- ReviewService
- ReportService

Rules:

- Coordinates workflows.
- Uses Domain and Infrastructure.
- Should not contain low-level persistence details.
- Should not contain direct console formatting logic unless unavoidable.

### Infrastructure

Contains data storage, seed data, and persistence.

Examples:

- AppDb
- SeedData
- JsonDataStore

Rules:

- Runtime data lives in in-memory collections.
- Persistence is handled through JSON files.
- Infrastructure should support loading on startup and saving on shutdown.

### Presentation

Contains console menus and user interaction.

Examples:

- MainMenu
- CustomerMenu
- AdministratorMenu
- Input helpers if needed

Rules:

- All menu navigation belongs here.
- Reads user input and displays output.
- Calls Application services to execute use cases.

## Folder Structure

- Domain/Entities
- Domain/Enums
- Application/Services
- Application/Helpers
- Infrastructure/Data
- Infrastructure/Persistence
- Presentation/Menus
- Common/Constants

## Design Guidance

- Keep the structure simple.
- Prefer direct service usage over unnecessary abstractions.
- Avoid generic repositories unless clearly helpful.
- Avoid introducing patterns just for appearance.

## Persistence Guidance

- Use JSON persistence.
- Persist Users, Products, and Orders.
- Keep persistence concerns isolated in Infrastructure.
- Avoid domain relationships that create serialization loops.
- Prefer IDs over deep bidirectional references where necessary.

## Feature Development Guidance

When implementing a feature:

1. identify the layer(s) involved
2. make the smallest possible change
3. preserve architectural boundaries
4. stop after a commit-sized chunk
