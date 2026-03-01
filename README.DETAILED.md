# ECommerceApp – ASP.NET Core Clean Architecture Backend

A production-oriented e-commerce backend built with ASP.NET Core, structured using Clean Architecture to demonstrate scalable, maintainable, and performance-aware backend design.


## 🎯 Purpose of This Project

This project was built to simulate a production-ready backend architecture and demonstrate:

- Clean layering and dependency management
- Transaction handling and consistency control
- Real-world authentication flows
- Scalable data access strategies
- Maintainable and testable service design

## 🚀 Tech Stack

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

## 🏗 Architecture

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

- Clear separation between Service and Repository layers
- DTO-based API contracts
- Soft delete implementation via BaseEntity
- AsNoTracking for read-only performance optimization
- CancellationToken propagation across layers
- Reusable paging and sorting extensions
- Redis-based distributed basket system
- Structured logging with request pipeline monitoring

## 🔐 Authentication & Authorization

- ASP.NET Core Identity integration
- Custom AppUser
- JWT Access & Refresh Token support
- Role-based authorization (Admin, StoreManager, Customer)
- Secure password handling via ASP.NET Identity
- Swagger JWT integration

## 🛍 Core Features

### 🧾 Product Management

- CRUD operations
- Pagination & sorting
- Soft delete optimization
- Performance-focused read queries

### 🗂 Category Module

- CRUD operations
- Validation rules
- Soft delete optimization

### 🖼 Product Image Management

- Cloudinary integration for external image storage
- PublicId persistence for external storage consistency
- Main image management logic
- Ownership validation checks

### 🧺 Redis-Based Basket System

- Distributed basket storage with Redis
- Add/update/merge item operations
- CancellationToken propagation

> Note: Basket-user association (persisting basket across login via token/cookie identity) and automatic basket migration (anonymous → authenticated) are planned.

## ⚙ Cross-Cutting Concerns

### Validation

- FluentValidation integration
- Extension-based automatic validation

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

- JWT authentication & role-based authorization
- Refresh token support

## 🧠 Design Principles Applied

- Clean Architecture layering
- Dependency Inversion Principle
- Repository Pattern
- Unit of Work Pattern
- Separation of Concerns
- Single Responsibility Principle
- Proper transaction boundaries at service level

## 🔄 Continuous Refactoring

The project evolves through structured refactoring such as:

- Soft delete optimization (partial property updates)
- CancellationToken propagation
- Core interface relocation
- AsNoTracking performance improvements
- Paging & sorting moved to reusable extensions
- Development configuration separation
- Structured logging across request pipeline and service layer
- The project was fully migrated to a proper Clean Architecture structure, separating:
	- API
	- Application
	- Domain
	- Infrastructure

with enforced dependency direction rules. 

## 📌 Roadmap

### 👤 User Management Enhancements

- User settings module
- Address management (multiple addresses support)
- Profile update flows
- Admin role management endpoints (promote/demote users)
- Role-based access policy refinements
  
### 🧾 Order & Payment Module

- Order creation workflow
- Order status management
- Payment integration structure (mock / provider-ready design)
- Transaction handling & consistency considerations

### 🧺 Basket Improvements

- Anonymous-to-authenticated basket migration
- Basket persistence across login (token-based association)
- Basket ownership validation

### 🖼 Image Management Improvements

- Background cleanup for orphaned Cloudinary files
- Retry mechanism for failed remote deletions
- Image processing pipeline (resize/compression)
  
### 📧 Email Integration

- Email service abstraction
- Registration confirmation email
- Password reset flow
- Order confirmation email

### 📦 Global API Improvements

- Standardized Global Response Wrapper
- Consistent success/error response structure
- Improved API versioning strategy
- Correlation ID tracking for distributed logging

### 🧪 Testing

- Unit tests (xUnit + FluentAssertions)
- Integration tests
- Test coverage improvement
- Authentication & authorization test scenarios

### 🐳 DevOps & Deployment

- Docker support
- CI/CD pipeline
- Environment-based configuration improvements

### ⚡ Performance & Caching

- Advanced caching strategies
- Cache invalidation patterns
- Query optimization improvements
- Redis-based distributed caching extensions

## 📦 API ENDPOINTS OVERVIEW

> All write operations require authentication and appropriate role authorization.

### 🔐 Authentication
- POST /api/auth/register
- POST /api/auth/login
- POST /api/auth/refresh-token

### 🧾 Products
- GET /api/products
- GET /api/products/{id}
- GET /api/products/category/{categoryId}
- POST /api/products
- PUT /api/products/{id}
- DELETE /api/products/{id}

### 🗂 Categories
- GET /api/categories
- GET /api/categories/{id}
- POST /api/categories
- PUT /api/categories/{id}
- DELETE /api/categories/{id}

### 🖼 Product Images
- POST /api/products/{id}/images
- PUT /api/products/{productId}/images/{imageId}/main
- DELETE /api/products/{productId}/images/{imageId}

### 🧺 Basket (Redis)
- GET /api/basket
- POST /api/basket/items
- DELETE /api/basket/{productId}

## ⚙️ INSTALLATION & RUNNING LOCALLY
### 📌 Requirements

- .NET 8 SDK
- SQL Server
- Redis
- Cloudinary account
  
### 1️⃣ Clone the repository

```
git clone https://github.com/your-username/ECommerceApp.git
cd ECommerceApp
```
### 2️⃣ Configure Database

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
