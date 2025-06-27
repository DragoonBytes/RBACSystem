using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RBACSystem.API.Models;

namespace RBACSystem.API.Endpoints;

public static class RoleManagementEndpoints
{
    public static void MapRoleManagementEndpoints(this WebApplication app)
    {
        var roleGroup = app.MapGroup("/api/roles").RequireAuthorization();

        roleGroup.MapGet("/", async (
            RoleManager<ApplicationRole> roleManager) =>
        {
            var roles = roleManager.Roles.ToList();
            return Results.Ok(roles);
        });

        roleGroup.MapPost("/", async (
            [FromBody] CreateRoleRequest request,
            RoleManager<ApplicationRole> roleManager) =>
        {
            var role = new ApplicationRole
            {
                Name = request.Name,
                Description = request.Description
            };

            var result = await roleManager.CreateAsync(role);

            if (!result.Succeeded)
            {
                return Results.BadRequest(result.Errors);
            }

            return Results.Ok(role);
        });

        roleGroup.MapPut("/{id}/permissions", async (
            string id,
            [FromBody] UpdatePermissionsRequest request,
            RoleManager<ApplicationRole> roleManager,
            ApplicationDbContext dbContext) =>
        {
            var role = await roleManager.FindByIdAsync(id);

            if (role == null)
            {
                return Results.NotFound();
            }

            // Obtener los permisos actuales del rol
            var currentPermissions = await dbContext.Permissions
                .Where(p => p.Roles.Any(r => r.Id == role.Id))
                .ToListAsync();

            // Obtener los nuevos permisos
            var newPermissions = await dbContext.Permissions
                .Where(p => request.PermissionIds.Contains(p.Id))
                .ToListAsync();

            // Eliminar permisos que no están en la nueva lista
            foreach (var permission in currentPermissions)
            {
                if (!newPermissions.Any(p => p.Id == permission.Id))
                {
                    permission.Roles.Remove(role);
                }
            }

            // Agregar nuevos permisos
            foreach (var permission in newPermissions)
            {
                if (!currentPermissions.Any(p => p.Id == permission.Id))
                {
                    permission.Roles.Add(role);
                }
            }

            await dbContext.SaveChangesAsync();

            return Results.Ok(new { Message = "Permissions updated successfully" });
        });
    }

    public class CreateRoleRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

    public class UpdatePermissionsRequest
    {
        public List<string> PermissionIds { get; set; } = new();
    }
}