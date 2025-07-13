namespace RBACSystem.API.Models;

public class Permission
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ICollection<ApplicationRole> Roles { get; set; } = new List<ApplicationRole>();
}