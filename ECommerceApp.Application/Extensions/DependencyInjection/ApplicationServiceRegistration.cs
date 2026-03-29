using ECommerceApp.Application.Common.Helpers;
using ECommerceApp.Application.Interfaces;
using ECommerceApp.Application.Mappings;
using ECommerceApp.Application.Services;
using ECommerceApp.Application.Validations.Products;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ECommerceApp.Application.Extensions.DependencyInjection
{
    public static class ApplicationServiceRegistration
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IOrderNumberGenerator, OrderNumberGenerator>();
            services.AddScoped<IUserAddressService, UserAddressService>();

            services.AddAutoMapper(_ => { }, typeof(MappingProfile).Assembly);
            services.AddValidatorsFromAssembly(typeof(ProductCreateDtoValidator).Assembly);
            return services;
        }
    }
}
