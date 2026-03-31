# ECommerceApp – ASP.NET Core Clean Architecture Backend

A production-oriented e-commerce backend built with ASP.NET Core and Clean Architecture, designed to demonstrate scalable, maintainable, and performance-aware backend development with real-world concerns such as authentication, validation, distributed caching, address-based checkout, reservation-aware order processing, and structured error handling.

## Purpose of This Project

This project was built to simulate a production-ready backend architecture and demonstrate:

- Clean layering and dependency boundaries
- Real-world authentication and authorization flows
- Maintainable service and repository design
- Validation-first application logic
- Distributed basket management with Redis
- Reservation-aware checkout and stock handling
- Structured logging and centralized exception handling
- Performance-focused data access patterns

## Tech Stack

- ASP.NET Core Web API
- Entity Framework Core (Code-First)
- ASP.NET Core Identity
- JWT Authentication (Access + Refresh Token)
- Redis (Basket management)
- Serilog (Structured Logging)
- FluentValidation
- AutoMapper
- Swagger (JWT integrated)
- SQL Server
- xUnit
- FluentAssertions
  
## Architecture

The project follows a layered Clean Architecture approach:

API → Application → Domain

Infrastructure → (Application abstractions + Domain)

      ┌───────────────┐
      │     API       │
      └──────┬────────┘
             │
             ▼
      ┌───────────────┐
      │  Application  │
      └──────┬────────┘
             │
             ▼
      ┌───────────────┐
      │    Domain     │
      └───────────────┘
             ▲
             │
      ┌───────────────┐
      │Infrastructure │
      └──────┬────────┘
             │
             └──────────────► (implements Application abstractions)

- **API** → Handles HTTP requests, controllers, middleware and dependency injection configuration.
- **Application** → Contains business use-cases, services, DTOs, validation logic, mapping profiles and external service abstractions.
- **Domain** → Core business entities, enums, domain exceptions and repository contracts.
- **Infrastructure** → Implements persistence (EF Core & Migrations), repositories, JWT generation and external integrations such as Redis and Cloudinary.
  
**Dependency Rule:**  
Dependencies flow inward. The Core layer remains framework-agnostic and independent.

### Key Architectural Decisions

- Clear separation between service and repository responsibilities
- DTO-based API contracts
- Soft delete support with global query filters
- AsNoTracking for read-only query optimization
- CancellationToken propagation across layers
- Reusable paging and sorting extensions
- Redis-based distributed basket storage
- Structured logging with request pipeline monitoring

## Authentication & Authorization

- ASP.NET Core Identity integration
- Custom AppUser
- JWT Access & Refresh Token support
- Role-based authorization (Admin, StoreManager, Customer)
- Refresh token rotation
- Refresh token hashing before persistence
- Logout / token revocation support
- Secure password handling via ASP.NET Identity
- Swagger JWT integration

## Core Features

### Product Management

- CRUD operations
- Pagination & sorting
- Validation support
- Soft delete strategy
- Performance-focused read queries

### Category Module

- CRUD operations
- Validation rules
- Soft delete support

### Product Image Management

- Cloudinary integration for external image storage
- PublicId persistence for external storage consistency
- Main image management logic
- Ownership validation checks

### User & Role Management

- Paginated user listing
- User detail retrieval
- User role listing
- Admin role assignment/removal endpoints
- Batch role loading to avoid N+1 queries in paginated user lists

### User Profile & Settings

- Authenticated profile retrieval
- Profile update support
- Password change support

### User Address Management
- Multiple address support per user
- Default address selection
- Ownership-based access control
- Address-based checkout support
  
### Redis-Based Basket System

- Distributed basket storage with Redis
- Add/update/remove item operations
- Cookie-based basket access
- CancellationToken propagation

> Note: Basket-user association (persisting basket across login via token/cookie identity) and automatic basket migration (anonymous → authenticated) are planned.

### Orders & Checkout

- Authenticated order creation
- Address-based checkout using saved user addresses
- Shipping address snapshot persistence on orders
- Reservation-based order creation with PendingPayment
- Reservation expiration support
- Stock confirmation on order approval
- Order detail and history endpoints
- Order status transition rules
- Cancellation flow for pending and confirmed orders

## Cross-Cutting Concerns

### Validation

- FluentValidation integration
- Validator discovery via assembly scanning
- Request-level business rule enforcement

### Error Handling

- Global exception middleware
- Custom exception types
- Standardized API error responses
- UTC-based timestamp consistency

### Logging

- Serilog integration
- Structured request logging
- File + console sinks

### Security

- JWT authentication 
- Role-based authorization
- Refresh token lifecycle handling

## Design Principles Applied

- Clean Architecture layering
- Dependency Inversion Principle
- Repository Pattern
- Unit of Work Pattern
- Separation of Concerns
- Single Responsibility Principle
- Proper transaction boundaries at service level

## Continuous Refactoring

The project evolves through structured refactoring such as:

- Soft delete optimization
- CancellationToken propagation
- AsNoTracking performance improvements
- Reusable paging and sorting abstractions
- Structured logging across request pipeline and service layer
- Address-based checkout design
- Reservation-aware order flow
- Profile/settings support
- Incremental migration toward cleaner dependency boundaries

## Roadmap

### User Management Enhancements

- Email confirmation flow
- Username/email change policies refinement
- Profile security improvements
- Role-based access policy refinements
  
### Order & Payment Module

- Payment provider integration
- Background expiration cleanup for pending payments
- Admin order management surface
- Stronger checkout consistency handling: transaction handling & consistency considerations

### Basket Improvements

- Anonymous-to-authenticated basket merge
- Basket persistence across login (token-based association)
- Basket ownership validation

### Image Management Improvements

- Background cleanup for orphaned Cloudinary files
- Retry mechanism for failed remote deletions
- Image processing pipeline (resize/compression)
  
### Email Integration

- Email service abstraction
- Registration confirmation email
- Password reset flow
- Order confirmation email

### Global API Improvements

- Standardized Global Response Wrapper
- Consistent success/error response structure
- Improved API versioning strategy
- Correlation ID tracking for distributed logging

### Testing

- Expanded service-level unit tests
- Integration tests for critical workflows
- Test coverage improvement
- Auth and checkout scenario coverage improvements

### DevOps & Deployment

- Docker support
- CI/CD pipeline
- Environment-based configuration improvements

### Performance & Caching

- Advanced caching strategies
- Cache invalidation patterns
- Query optimization improvements
- Redis-based distributed caching extensions

### Testing

The solution includes an application test project for critical business logic.

Run tests with:
```
dotnet test
```

## API ENDPOINTS OVERVIEW

> All write operations require authentication and appropriate role authorization.

### Authentication
- POST /api/auth/register
- POST /api/auth/login
- POST /api/auth/refresh-token
- POST /api/auth/logout

### Products
- GET /api/products
- GET /api/products/{id}
- GET /api/products/category/{categoryId}
- POST /api/products
- PUT /api/products/{id}
- DELETE /api/products/{id}

### Categories
- GET /api/categories
- GET /api/categories/{id}
- POST /api/categories
- PUT /api/categories/{id}
- DELETE /api/categories/{id}

### Product Images
- POST /api/products/{id}/images
- PUT /api/products/{productId}/images/{imageId}/main
- DELETE /api/products/{productId}/images/{imageId}

### Users
- GET /api/users
- GET /api/users/{userId}
- GET /api/users/{userId}/roles
- POST /api/users/{userId}/roles
- DELETE /api/users/{userId}/roles/{roleName}

### User Profile
- GET /api/users/me
- PUT /api/users/me
- POST /api/users/change-password

### User Addresses
- GET /api/useraddresses
- GET /api/useraddresses/{id}
- POST /api/useraddresses
- PUT /api/useraddresses/{id}
- PATCH /api/useraddresses/{id}/set-default
- DELETE /api/useraddresses/{id}
  
### Basket (Redis)
- GET /api/basket
- POST /api/basket/items
- DELETE /api/basket/{productId}

### Orders
- POST /api/orders
- GET /api/orders
- GET /api/orders/{orderId}
- POST /api/orders/{orderId}/cancel
- PATCH /api/orders/{orderId}/status

## INSTALLATION & RUNNING LOCALLY
### Requirements

- .NET 8 SDK
- SQL Server
- Redis
- Cloudinary account
  
### Clone the repository

```
git clone https://github.com/your-username/ECommerceApp.git
cd ECommerceApp
```
### Configure Database

Update the connection string in:

appsettings.Development.json

```
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=ECommerceAppDb;Trusted_Connection=True;TrustServerCertificate=True;"
  },
  "JwtSettings": {
    "SecurityKey": "your-super-secret-key",
    "Issuer": "ECommerceApp",
    "Audience": "ECommerceAppUsers",
    "AccessTokenExpiration": 30,
    "RefreshTokenExpiration": 7
  },
  "CloudinaryOptions": {
    "CloudName": "your-cloud-name",
    "ApiKey": "your-api-key",
    "ApiSecret": "your-api-secret"
  },
  "CheckoutSettings": {
    "ReservationTimeoutMinutes": 15
  }
}
```

### 3️⃣ Apply Database Migrations

Then apply migrations:
Make sure EF CLI is installed:

```
dotnet tool install --global dotnet-ef
dotnet ef database update --project ECommerceApp.Infrastructure --startup-project ECommerceApp.API
```

### 4️⃣ Configure Redis

Make sure Redis is running locally (default port: 6379).

### 5️⃣ Run the Application
```
dotnet run --project ECommerceApp.API
```
The API will be available at:

https://localhost:xxxx/swagger
