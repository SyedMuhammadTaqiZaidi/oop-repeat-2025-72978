using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WorkshopSystem.Core.Domain.Enums;
using Microsoft.Extensions.DependencyInjection;
using WorkshopSystem.Core.Domain.Entities;

namespace WorkshopSystem.Infrastructure.Data
{
    public static class DatabaseSeeder
    {
        public static async Task SeedData(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var services = scope.ServiceProvider;
            var context = services.GetRequiredService<ApplicationDbContext>();
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

            // Apply any pending migrations
            await context.Database.MigrateAsync();

            // Seed roles
            await SeedRoles(roleManager);
            
            // Seed admin user
            await SeedAdminUser(userManager);
            
            // Seed mechanics
            await SeedMechanics(userManager);
            
            // Seed customers
            await SeedCustomers(userManager);
            
            // Seed cars
            await SeedCars(context, userManager);
            
            // Seed service records
            await SeedServiceRecords(context, userManager);
        }

        private static async Task SeedRoles(RoleManager<IdentityRole> roleManager)
        {
            string[] roles = { "Admin", "Mechanic", "Customer" };

            foreach (var role in roles)
            {
                var roleExists = await roleManager.RoleExistsAsync(role);
                if (!roleExists)
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }

        private static async Task SeedAdminUser(UserManager<ApplicationUser> userManager)
        {
            var adminEmail = "admin@carservice.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            
            if (adminUser == null)
            {
                var admin = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FirstName = "Admin",
                    LastName = "User",
                    EmailConfirmed = true
                };

                var createAdmin = await userManager.CreateAsync(admin, "Dorset001^");
                if (createAdmin.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, "Admin");
                }
            }
        }

        private static async Task SeedMechanics(UserManager<ApplicationUser> userManager)
        {
            var mechanic1Email = "mechanic1@carservice.com";
            var mechanic1 = await userManager.FindByEmailAsync(mechanic1Email);
            
            if (mechanic1 == null)
            {
                var mechanic = new Mechanic
                {
                    UserName = mechanic1Email,
                    Email = mechanic1Email,
                    FirstName = "John",
                    LastName = "Doe",
                    EmailConfirmed = true
                };

                var createMechanic = await userManager.CreateAsync(mechanic, "Dorset001^");
                if (createMechanic.Succeeded)
                {
                    await userManager.AddToRoleAsync(mechanic, "Mechanic");
                }
            }

            var mechanic2Email = "mechanic2@carservice.com";
            var mechanic2 = await userManager.FindByEmailAsync(mechanic2Email);
            
            if (mechanic2 == null)
            {
                var mechanic = new Mechanic
                {
                    UserName = mechanic2Email,
                    Email = mechanic2Email,
                    FirstName = "Jane",
                    LastName = "Smith",
                    EmailConfirmed = true
                };

                var createMechanic = await userManager.CreateAsync(mechanic, "Dorset001^");
                if (createMechanic.Succeeded)
                {
                    await userManager.AddToRoleAsync(mechanic, "Mechanic");
                }
            }
        }

        private static async Task SeedCustomers(UserManager<ApplicationUser> userManager)
        {
            var customer1Email = "customer1@carservice.com";
            var customer1 = await userManager.FindByEmailAsync(customer1Email);
            
            if (customer1 == null)
            {
                customer1 = new Customer
                {
                    UserName = customer1Email,
                    Email = customer1Email,
                    FirstName = "Alice",
                    LastName = "Johnson",
                    EmailConfirmed = true,
                    Address = "123 Main St",
                    PhoneNumber = "1234567890"
                };

                var createCustomer = await userManager.CreateAsync(customer1, "Dorset001^");
                if (createCustomer.Succeeded)
                {
                    await userManager.AddToRoleAsync(customer1, "Customer");
                }
            }

            var customer2Email = "customer2@carservice.com";
            var customer2 = await userManager.FindByEmailAsync(customer2Email);
            
            if (customer2 == null)
            {
                customer2 = new Customer
                {
                    UserName = customer2Email,
                    Email = customer2Email,
                    FirstName = "Bob",
                    LastName = "Williams",
                    EmailConfirmed = true,
                    Address = "456 Elm St",
                    PhoneNumber = "9876543210"
                };

                var createCustomer = await userManager.CreateAsync(customer2, "Dorset001^");
                if (createCustomer.Succeeded)
                {
                    await userManager.AddToRoleAsync(customer2, "Customer");
                }
            }
        }

        private static async Task SeedCars(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            if (await context.Cars.AnyAsync()) return;

            var customer1 = await userManager.FindByEmailAsync("customer1@carservice.com");
            var customer2 = await userManager.FindByEmailAsync("customer2@carservice.com");

            if (customer1 == null || customer2 == null) return;

            var cars = new[]
            {
                new Car
                {
                    RegistrationNumber = "ABC123",
                    CustomerId = customer1.Id
                },
                new Car
                {
                    RegistrationNumber = "XYZ789",
                    CustomerId = customer1.Id
                },
                new Car
                {
                    RegistrationNumber = "DEF456",
                    CustomerId = customer2.Id
                }
            };

            await context.Cars.AddRangeAsync(cars);
            await context.SaveChangesAsync();
        }

        private static async Task SeedServiceRecords(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            if (await context.ServiceRecords.AnyAsync()) return;

            var admin = await userManager.FindByEmailAsync("admin@carservice.com");
            var mechanic1 = await userManager.FindByEmailAsync("mechanic1@carservice.com");
            var customer1 = await userManager.FindByEmailAsync("customer1@carservice.com");

            if (admin == null || mechanic1 == null || customer1 == null) return;

            // Get the first car for customer1
            var customer1Car = await context.Cars.FirstOrDefaultAsync(c => c.CustomerId == customer1.Id);
            if (customer1Car == null) return;

            var serviceRecords = new[]
            {
                new ServiceRecord
                {
                    ServiceDate = DateTime.Now.AddDays(-7),
                    Description = "Regular maintenance and oil change",
                    HoursWorked = 1.5,
                    Status = ServiceStatus.Completed,
                    CompletionDate = DateTime.Now.AddDays(-6),
                    CustomerId = customer1.Id,
                    CarId = customer1Car.Id,
                    AssignedMechanicId = mechanic1.Id,
                    RequestedById = admin.Id
                },
                new ServiceRecord
                {
                    ServiceDate = DateTime.Now,
                    Description = "Brake system check",
                    HoursWorked = 0,
                    Status = ServiceStatus.InProgress,
                    CustomerId = customer1.Id,
                    CarId = customer1Car.Id,
                    AssignedMechanicId = mechanic1.Id,
                    RequestedById = admin.Id
                }
            };

            await context.ServiceRecords.AddRangeAsync(serviceRecords);
            await context.SaveChangesAsync();
        }
    }
}
