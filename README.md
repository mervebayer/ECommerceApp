# ECommerceApp – ASP.NET Core Clean Architecture Backend

![.NET](https://img.shields.io/badge/.NET-8.0-purple)
![Architecture](https://img.shields.io/badge/architecture-clean--architecture-blue)
![Status](https://img.shields.io/badge/status-actively%20maintained-brightgreen)

Production-oriented e-commerce backend built with **ASP.NET Core** and **Clean Architecture**, focusing on real-world backend concerns: **auth flows**, **caching**, **logging**, **error handling**, and **performance-aware data access**.

## 🎯 Purpose
Built to demonstrate:
- Clean layering & dependency boundaries (Domain / Application / Infrastructure / API)
- Token-based authentication (JWT access + refresh) with role-based authorization
- Centralized exception handling via custom middleware
- Redis-based distributed basket design
- Maintainable service/repository structure with validation & logging
- Soft delete strategy with global query filters
- Performance-aware data access patterns (pagination, sorting, AsNoTracking)

## 🧰 Tech Stack
ASP.NET Core Web API • EF Core • Identity • JWT • Redis • Serilog • FluentValidation • AutoMapper • Swagger • SQL Server

## 🧱 Architecture

**API → Application → Domain**  
**Infrastructure → (Application abstractions + Domain)**  
> Dependency flow is inward.

## ✅ Core Features
- **Auth**: ASP.NET Identity, JWT access/refresh, roles (Admin/StoreManager/Customer), Swagger JWT
- **Products & Categories**: CRUD, pagination & sorting, validation, soft delete
- **Images**: Cloudinary storage, PublicId persistence, main image logic
- **Basket**: Redis distributed basket operations


## ▶ Run Locally (Quick)
1) Configure `appsettings.Development.json` (SQL, Redis, JwtSettings, CloudinaryOptions)  
2) Apply migrations:
```
dotnet tool install --global dotnet-ef
dotnet ef database update --project ECommerceApp.Infrastructure --startup-project ECommerceApp.API
```
3) Run
```
dotnet run --project ECommerceApp.API
```
