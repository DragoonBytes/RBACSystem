namespace RBACSystem.Blazor.Models;

public class LoginModel
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class RegisterModel
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
}

public class LoginResult
{
    public bool Success { get; set; }
    public string? Error { get; set; }
    public string? Token { get; set; }
    public DateTime Expiration { get; set; }
    public UserInfo? User { get; set; }
}

public class RegisterResult
{
    public bool Success { get; set; }
    public IEnumerable<string>? Errors { get; set; }
}