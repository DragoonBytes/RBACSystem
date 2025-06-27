using System.Net.Http.Json;
using RBACSystem.Blazor.Models;

namespace RBACSystem.Blazor.Services;

public class AuthService
{
    private readonly HttpClient _httpClient;

    public AuthService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<LoginResult> Login(LoginModel loginModel)
    {
        var response = await _httpClient.PostAsJsonAsync("api/auth/login", loginModel);

        if (!response.IsSuccessStatusCode)
        {
            return new LoginResult { Success = false, Error = "Invalid login attempt" };
        }

        var result = await response.Content.ReadFromJsonAsync<LoginResult>();
        return result ?? new LoginResult { Success = false, Error = "Invalid response from server" };
    }

    public async Task<RegisterResult> Register(RegisterModel registerModel)
    {
        var response = await _httpClient.PostAsJsonAsync("api/auth/register", registerModel);

        if (!response.IsSuccessStatusCode)
        {
            var errors = await response.Content.ReadFromJsonAsync<RegisterResult>();
            return errors ?? new RegisterResult { Success = false, Errors = new[] { "Unknown error occurred" } };
        }

        return new RegisterResult { Success = true };
    }
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

public class UserInfo
{
    public string? Id { get; set; }
    public string? Email { get; set; }
    public string? UserName { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public IEnumerable<string>? Roles { get; set; }
}