# ECommerceApp – ASP.NET Core Clean Architecture Backend

![.NET](https://img.shields.io/badge/.NET-8.0-purple)
![Architecture](https://img.shields.io/badge/architecture-clean--architecture-blue)
![Status](https://img.shields.io/badge/status-actively%20maintained-brightgreen)

Production-oriented e-commerce backend built with **ASP.NET Core** and **Clean Architecture**, focusing on real-world backend concerns such as **authentication**, **validation**, **caching**, **logging**, **exception handling**, **reservation-based checkout**, ** payment processing**, **real-time notifications**, **email verification**, and **performance-aware data access**.


## Purpose
Built to demonstrate:
- Clean layering & dependency boundaries (Domain / Application / Infrastructure / API)
- Token-based authentication (JWT access + refresh) with role-based authorization
- Email verification with SMTP-based email delivery
- Centralized exception handling via custom middleware
- Redis-based distributed basket design
- Maintainable service/repository structure with validation & logging
- Soft delete strategy with global query filters
- Performance-aware data access patterns (pagination, sorting, AsNoTracking)
- Reservation-aware order creation flow with address-based checkout
- Payment transaction tracking and idempotent payment processing
- Real-time notification delivery with SignalR
- AI-assisted product description generation

## Tech Stack
ASP.NET Core Web API • EF Core • Identity • JWT • Redis • SignalR • Serilog • FluentValidation • AutoMapper • Swagger • SQL Server • SMTP • Cloudinary • Iyzico Sandbox

## Architecture

**API → Application → Domain**  
**Infrastructure → (Application abstractions + Domain)**  
> Dependency flow is inward.

## Core Features
- **Auth**: ASP.NET Identity, JWT access/refresh tokens, role-based authorization (`Admin / StoreManager / Customer`)
- **Email Verification:** SMTP-based email sending and account verification flow
- **Products & Categories**: CRUD operations, pagination, sorting, validation, soft delete
- **AI Product Descriptions:** Product description field with AI-powered product description generation endpoint
- **Images**: Cloudinary integration, `PublicId` persistence, main image management
- **Basket**: Redis-based distributed basket operations with improved basket identity handling for authenticated users
- **User Addresses**: Multiple address management, default address selection, ownership-based access control
- **Orders**: Address-based checkout, shipping address snapshot persistence, order history/detail endpoints
- **Checkout Flow**: Reservation-based order creation with `PendingPayment`, expiration handling, stock confirmation on order approval
- **Payment Processing:** Iyzico non-3DS sandbox payment integration, payment transaction tracking, and idempotent payment flow
- **Notifications:** In-app notifications for new orders and status updates, with separate backoffice and customer notification audiences
- **Real-Time Notifications:** SignalR-based real-time notification delivery
- **User Profile**: Profile update and password change support
- **Favorites**: Authenticated user favorites management with add, remove, list, and favorite status checks
- **Logging & Error Handling**: Centralized exception middleware and structured logging

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
