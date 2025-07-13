using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RBACSystem.API.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RBACSystem.API.Endpoints;

public static class UserManagementEndpoints
{
    public static void MapUserManagementEndpoints(this WebApplication app)
    {
        var userGroup = app.MapGroup("/api/users").RequireAuthorization();


        userGroup.MapPut("/{id}/roles", async (
            string id,
            [FromBody] UpdateRolesRequest request,
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager) =>
        {
            var user = await userManager.FindByIdAsync(id);
            if (user == null) return Results.NotFound();

            // Validar que los roles existan
            foreach (var roleName in request.Roles)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    return Results.BadRequest($"Role '{roleName}' does not exist");
                }
            }

            // Obtener los roles actuales del usuario
            var currentRoles = await userManager.GetRolesAsync(user);

            // Eliminar roles que no están en la nueva lista
            var rolesToRemove = currentRoles.Except(request.Roles);
            await userManager.RemoveFromRolesAsync(user, rolesToRemove);

            // Agregar nuevos roles
            var rolesToAdd = request.Roles.Except(currentRoles);
            await userManager.AddToRolesAsync(user, rolesToAdd);

            return Results.Ok(new { Message = "Roles updated successfully" });
        });
    }

    public class UpdateRolesRequest
    {
        public List<string> Roles { get; set; } = new List<string>();
    }
}