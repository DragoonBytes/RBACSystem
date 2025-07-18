﻿namespace RBACSystem.Blazor.Models;

public class UserInfo
{
    public string? Id { get; set; }
    public string? Email { get; set; }
    public string? UserName { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public IEnumerable<string>? Roles { get; set; }
}