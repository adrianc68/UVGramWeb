using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using UVGramWeb;
using UVGramWeb.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Blazored.Modal;
using UVGramWeb.Helpers; 


var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

ConfigHelper.Initialize(builder.Configuration);

builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

var apiBaseUrl = builder.Configuration["ApiSettings:BaseUrl"];
var apiUrl = new Uri(apiBaseUrl);


builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, ApiAuthenticationStateProvider>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<IHttpService, HttpService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<ILocalStorageService, LocalStorageService>();
builder.Services.AddScoped<IPostService, PostService>();
builder.Services.AddScoped<IChatService, ChatService>();
builder.Services.AddBlazoredModal();


builder.Services.AddScoped(sp => new HttpClient { BaseAddress = apiUrl });



var host = builder.Build();
var authService = host.Services.GetRequiredService<IAuthenticationService>();
await authService.Initialize();

await host.RunAsync();
