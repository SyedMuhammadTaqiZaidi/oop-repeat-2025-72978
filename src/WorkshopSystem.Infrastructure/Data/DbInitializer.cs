using Microsoft.AspNetCore.Identity;
using WorkshopSystem.Core.Domain.Entities;

namespace WorkshopSystem.Infrastructure.Data
{
    public static class DbInitializer
    {
        public static async Task InitializeAsync(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            // Create roles if they don't exist
            string[] roles = { "Admin", "Mechanic", "Customer" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // Create admin user if it doesn't exist
            var adminEmail = "admin@workshop.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FirstName = "Admin",
                    LastName = "User",
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(adminUser, "Admin123!");
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }

            // Create mechanic user if it doesn't exist
            var mechanicEmail = "mechanic@workshop.com";
            var mechanicUser = await userManager.FindByEmailAsync(mechanicEmail);
            if (mechanicUser == null)
            {
                mechanicUser = new ApplicationUser
                {
                    UserName = mechanicEmail,
                    Email = mechanicEmail,
                    FirstName = "John",
                    LastName = "Mechanic",
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(mechanicUser, "Mechanic123!");
                await userManager.AddToRoleAsync(mechanicUser, "Mechanic");
            }

            // Create customer user if it doesn't exist
            var customerEmail = "customer@workshop.com";
            var customerUser = await userManager.FindByEmailAsync(customerEmail);
            if (customerUser == null)
            {
                customerUser = new ApplicationUser
                {
                    UserName = customerEmail,
                    Email = customerEmail,
                    FirstName = "Jane",
                    LastName = "Customer",
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(customerUser, "Customer123!");
                await userManager.AddToRoleAsync(customerUser, "Customer");
            }
        }
    }
} 