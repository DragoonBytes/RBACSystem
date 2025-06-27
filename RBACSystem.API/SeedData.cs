using Microsoft.AspNetCore.Identity;
using RBACSystem.API.Models;

namespace RBACSystem.API;

public static class SeedData
{
    public static async Task Initialize(IServiceProvider serviceProvider)
    {
        var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = serviceProvider.GetRequiredService<RoleManager<ApplicationRole>>();

        // Crear roles si no existen
        string[] roleNames = { "Admin", "Manager", "User" };

        foreach (var roleName in roleNames)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new ApplicationRole
                {
                    Name = roleName,
                    Description = $"{roleName} role"
                });
            }
        }

        // Crear permisos si no existen
        if (!context.Permissions.Any())
        {
            var permissions = new List<Permission>
            {
                new() { Name = "users.read", Description = "Read users" },
                new() { Name = "users.create", Description = "Create users" },
                new() { Name = "users.update", Description = "Update users" },
                new() { Name = "users.delete", Description = "Delete users" },
                new() { Name = "roles.manage", Description = "Manage roles" },
                new() { Name = "permissions.manage", Description = "Manage permissions" }
            };

            await context.Permissions.AddRangeAsync(permissions);
            await context.SaveChangesAsync();
        }

        // Crear usuario admin si no existe
        var adminEmail = "admin@example.com";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);

        if (adminUser == null)
        {
            adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                FirstName = "Admin",
                LastName = "User"
            };

            var result = await userManager.CreateAsync(adminUser, "Admin123!");

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");

                // Asignar todos los permisos al rol Admin
                var adminRole = await roleManager.FindByNameAsync("Admin");
                var allPermissions = context.Permissions.ToList();

                foreach (var permission in allPermissions)
                {
                    permission.Roles.Add(adminRole);
                }

                await context.SaveChangesAsync();
            }
        }
    }
}