using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace RBACSystem.API.Models;

public class ApplicationRole : IdentityRole
{
    public string? Description { get; set; }

    public ICollection<Permission> Permissions { get; set; } = new List<Permission>();
}