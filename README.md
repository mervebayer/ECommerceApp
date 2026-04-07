# ECommerceApp – ASP.NET Core Clean Architecture Backend

![.NET](https://img.shields.io/badge/.NET-8.0-purple)
![Architecture](https://img.shields.io/badge/architecture-clean--architecture-blue)
![Status](https://img.shields.io/badge/status-actively%20maintained-brightgreen)

Production-oriented e-commerce backend built with **ASP.NET Core** and **Clean Architecture**, focusing on real-world backend concerns such as **authentication**, **validation**, **caching**, **logging**, **exception handling**, **reservation-based checkout**, and **performance-aware data access**.

## Purpose
Built to demonstrate:
- Clean layering & dependency boundaries (Domain / Application / Infrastructure / API)
- Token-based authentication (JWT access + refresh) with role-based authorization
- Centralized exception handling via custom middleware
- Redis-based distributed basket design
- Maintainable service/repository structure with validation & logging
- Soft delete strategy with global query filters
- Performance-aware data access patterns (pagination, sorting, AsNoTracking)
- Reservation-aware order creation flow with address-based checkout
  
## Tech Stack
ASP.NET Core Web API • EF Core • Identity • JWT • Redis • Serilog • FluentValidation • AutoMapper • Swagger • SQL Server

## Architecture

**API → Application → Domain**  
**Infrastructure → (Application abstractions + Domain)**  
> Dependency flow is inward.

## Core Features
- **Auth**: ASP.NET Identity, JWT access/refresh tokens, role-based authorization (`Admin / StoreManager / Customer`)
- **Products & Categories**: CRUD operations, pagination, sorting, validation, soft delete
- **Images**: Cloudinary integration, `PublicId` persistence, main image management
- **Basket**: Redis-based distributed basket operations
- **User Addresses**: multiple address management, default address selection, ownership-based access control
- **Orders**: address-based checkout, shipping address snapshot persistence, order history/detail endpoints
- **Checkout Flow**: reservation-based order creation with `PendingPayment`, expiration handling, stock confirmation on order approval
- **User Profile**: profile update and password change support
- **Favorites**: authenticated user favorites management with add, remove, list, and favorite status checks
- **Logging & Error Handling**: centralized exception middleware and structured logging

## Testing

Run tests with:
```
dotnet test
```

The solution includes application-level tests for critical business logic.

## Run Locally (Quick)
1) Configure `appsettings.Development.json` (SQL connection, Redis, JwtSettings, CloudinaryOptions, CheckoutSettings)  
2) Apply migrations:
```
dotnet tool install --global dotnet-ef
dotnet ef database update --project ECommerceApp.Infrastructure --startup-project ECommerceApp.API
```
3) Run
```
dotnet run --project ECommerceApp.API
```
