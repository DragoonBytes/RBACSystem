using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using RBACSystem.Blazor;
using RBACSystem.Blazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Configure HttpClient
builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri("https://localhost:7195") // Asegúrate de que esta URL coincida con tu API
});

// Add services
builder.Services.AddMudServices();
builder.Services.AddScoped<ILocalStorageService, LocalStorageService>();
builder.Services.AddScoped<AuthInterceptor>();
builder.Services.AddScoped<AuthService>();

// Configure HTTP client with auth interceptor
builder.Services.AddHttpClient("AuthHttpClient", client =>
{
    client.BaseAddress = new Uri("https://localhost:7195");
}).AddHttpMessageHandler<AuthInterceptor>();

builder.Services.AddScoped(sp =>
    sp.GetRequiredService<IHttpClientFactory>().CreateClient("AuthHttpClient"));

// Configure authentication
builder.Services.AddOptions();
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();

await builder.Build().RunAsync();