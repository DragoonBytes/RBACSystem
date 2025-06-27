using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RBACSystem.API.Models;

namespace RBACSystem.API.Endpoints;

public static class PermissionEndpoints
{
    public static void MapPermissionEndpoints(this WebApplication app)
    {
        var permissionGroup = app.MapGroup("/api/permissions").RequireAuthorization();

        permissionGroup.MapGet("/", async (ApplicationDbContext dbContext) =>
        {
            var permissions = await dbContext.Permissions.ToListAsync();
            return Results.Ok(permissions);
        });

        permissionGroup.MapPost("/", async (
            [FromBody] CreatePermissionRequest request,
            ApplicationDbContext dbContext) =>
        {
            var permission = new Permission
            {
                Name = request.Name,
                Description = request.Description
            };

            dbContext.Permissions.Add(permission);
            await dbContext.SaveChangesAsync();

            return Results.Ok(permission);
        });

        permissionGroup.MapGet("/{roleId}", async (
            string roleId,
            ApplicationDbContext dbContext) =>
        {
            var permissions = await dbContext.Permissions
                .Where(p => p.Roles.Any(r => r.Id == roleId))
                .ToListAsync();

            return Results.Ok(permissions);
        });
    }

    public class CreatePermissionRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}