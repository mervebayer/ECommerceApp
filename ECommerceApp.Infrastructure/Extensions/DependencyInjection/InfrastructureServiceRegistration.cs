using ECommerceApp.Application.Interfaces;
using ECommerceApp.Application.Services;
using ECommerceApp.Domain.Entities;
using ECommerceApp.Domain.Interfaces;
using ECommerceApp.Domain.Interfaces.Repositories;
using ECommerceApp.Infrastructure.ExternalServices;
using ECommerceApp.Infrastructure.Interfaces.Repositories;
using ECommerceApp.Infrastructure.Models;
using ECommerceApp.Infrastructure.Options;
using ECommerceApp.Infrastructure.Persistence;
using ECommerceApp.Infrastructure.Repositories;
using ECommerceApp.Infrastructure.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApp.Infrastructure.Extensions.DependencyInjection
{
    public static class InfrastructureServiceRegistration
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // DbContext
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            // Identity
            services.AddIdentity<AppUser, IdentityRole>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

            // UoW + Repos
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IProductImageRepository, ProductImageRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IProductReadRepository, ProductReadRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IUserAddressRepository, UserAddressRepository>();
            services.AddScoped<IFavoriteRepository, FavoriteRepository>();

            // Cloudinary
            services.Configure<CloudinaryOptions>(configuration.GetSection("Cloudinary"));
            services.AddScoped<IImageStorage, CloudinaryImageStorage>();
            services.AddScoped<IProductImageService, ProductImageService>();

            // Redis
            var redisConnectionString = configuration.GetConnectionString("Redis");
            services.AddSingleton<IConnectionMultiplexer>(_ => ConnectionMultiplexer.Connect(redisConnectionString));
            services.AddScoped<IBasketRepository, BasketRepository>();
            services.AddScoped<IBasketService, BasketService>();

            // JWT settings binding (options)
            services.Configure<JwtSettings>(configuration.GetSection("JWTSettings"));
            services.AddSingleton<IJwtProvider, JwtProvider>();

            services.Configure<CheckoutSettings>(configuration.GetSection("CheckoutSettings"));
            services.AddScoped<ICheckoutSettings>(sp =>sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<CheckoutSettings>>().Value);

            return services;
        }
    }
}
