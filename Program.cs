using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using UVGramWeb;
using UVGramWeb.Shared;
using UVGramWeb.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<IHttpService, HttpService>();
builder.Services.AddScoped<ILocalStorageService, LocalStorageService>();
builder.Services.AddScoped(x =>
{
    var apiUrl = new Uri("http://192.168.1.67:8080");
    return new HttpClient() { BaseAddress = apiUrl };
});

builder.Services.AddSingleton<PageHistoryState>();

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("http://192.168.1.67:8080") });

var host = builder.Build();
var authService = host.Services.GetRequiredService<IAuthenticationService>();
await authService.Initialize();

await host.RunAsync();
