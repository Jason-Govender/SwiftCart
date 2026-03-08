# Skill: Lightweight DDD for C# Console Shopping App

Always read `spec.md` before applying this skill.

## Purpose

Use this skill when implementing features for the shopping backend console application.

The goal is to apply Domain-Driven Design in a practical, lightweight way that supports small issue-based implementation and commit-sized code generation.

## Project Context

- C# console application
- Online shopping backend simulation
- Books are the chosen product domain unless `spec.md` says otherwise
- Submission 1 is the immediate target
- JSON persistence exists but must remain simple
- Work must stay small and reviewable

## Responsibilities By Layer

### Domain

Represents business concepts.

Examples:

- Customer owns a Cart
- Product has Name, Price, Stock, and Reviews
- Order contains OrderItems and status
- Payment represents checkout payment information

Do:

- model the business clearly
- keep classes focused
- use enums where appropriate

Do not:

- read from the console
- write to the console
- save files
- coordinate workflows

### Application

Represents business use cases.

Examples:

- register customer
- login user
- browse products
- search products
- add to cart
- checkout
- generate reports
- restock product

Do:

- orchestrate use cases
- call Infrastructure when needed
- return data/results cleanly

Do not:

- contain menu printing logic
- manage raw file serialization directly

### Infrastructure

Represents storage and persistence.

Examples:

- in-memory runtime data
- seed data
- JSON load/save

Do:

- isolate persistence
- support app startup/shutdown data flow

Do not:

- leak file concerns into Domain

### Presentation

Represents menu-driven interaction.

Examples:

- main menu
- customer menu
- admin menu

Do:

- show numbered options
- collect input
- call services
- display results

Do not:

- hide business logic inside menus

## Chunking Rule

When asked to implement a feature, always break it into the smallest useful slices.

Preferred order:

1. define/update domain model
2. add service logic
3. wire menu flow
4. add validation
5. polish output if needed

Do not output all slices at once unless explicitly requested.

## Commit-Sized Behavior

Each response should aim to support one commit.
Examples:

- add Product entity
- add UserRole enum
- scaffold AuthService
- wire login option into MainMenu
- add SaveAll method to JsonDataStore

## Example Interpretation

If asked:
"Implement cart functionality"

Do not generate the whole cart system.

Instead propose:

1. create Cart and CartItem entities
2. add AddToCart method in CartService
3. wire Add Product to Cart option in CustomerMenu
4. add validation for stock and quantity
5. add View Cart flow

Then generate only step 1 unless asked for more.

## Output Rule

For each request:

- mention the part of `spec.md` being satisfied
- state the files to be changed
- produce only one chunk
- suggest a commit message
