using ECommerceApp.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApp.Infrastructure.Persistence
{
    public static class IdentitySeedData
    {
        public static async Task SeedAsync(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
        {
            await CreateRolesAsync(roleManager);

            await CreateAdminUserAsync(userManager,configuration);
        }

        private static async Task CreateRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            string[] roles = { "Admin", "StoreManager", "Customer" };

            foreach (var roleName in roles)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
        }

        private static async Task CreateAdminUserAsync(UserManager<AppUser> userManager, IConfiguration configuration)
        {
            var adminEmail = "admin@ecommerce.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                adminUser = new AppUser
                {
                    UserName = "admin",
                    Email = adminEmail,
                    EmailConfirmed = true,
                    FirstName = "System",
                    LastName = "Administrator"
                };
                var password = configuration["AdminPassword"];
                var result = await userManager.CreateAsync(adminUser, password ?? "AdminPass123!");
                if (result.Succeeded)
                {
                
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }
        }
    }
}