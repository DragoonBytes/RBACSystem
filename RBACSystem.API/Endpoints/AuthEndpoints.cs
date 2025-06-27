using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RBACSystem.API.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace RBACSystem.API.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this WebApplication app)
    {
        var authGroup = app.MapGroup("/api/auth");

        authGroup.MapPost("/register", async (
            [FromBody] RegisterRequest request,
            UserManager<ApplicationUser> userManager) =>
        {
            var user = new ApplicationUser
            {
                UserName = request.Email,
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName
            };

            var result = await userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded)
            {
                return Results.BadRequest(result.Errors);
            }

            // Asignar rol de usuario por defecto
            await userManager.AddToRoleAsync(user, "User");

            return Results.Ok(new { Message = "User registered successfully" });
        });

        authGroup.MapPost("/login", async (
            [FromBody] LoginRequest request,
            UserManager<ApplicationUser> userManager,
            IConfiguration configuration) =>
        {
            var user = await userManager.FindByEmailAsync(request.Email);

            if (user == null || !await userManager.CheckPasswordAsync(user, request.Password))
            {
                return Results.Unauthorized();
            }

            var roles = await userManager.GetRolesAsync(user);

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id),
                new(ClaimTypes.Email, user.Email),
                new(ClaimTypes.Name, user.UserName),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var jwtSettings = configuration.GetSection("JwtSettings");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Secret"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddMinutes(Convert.ToDouble(jwtSettings["ExpirationInMinutes"]));

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );

            return Results.Ok(new
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiration = expires,
                User = new
                {
                    user.Id,
                    user.Email,
                    user.UserName,
                    user.FirstName,
                    user.LastName,
                    Roles = roles
                }
            });
        });
    }

    public class RegisterRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
    }

    public class LoginRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}