using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Shared.Authorization;
using Shared.Helpers;
using Shared.Providers;
using ReserveMe;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient
{
	BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
});

builder.Services.AddBlazoredLocalStorage();

// API provider
builder.Services.AddScoped<IApiProvider, ApiProvider>();

// Authentication helper
builder.Services.AddScoped<IAuthenticationHelper, AuthenticationHelper>();
builder.Services.AddScoped<AuthenticationStateProvider, ApiAuthenticationStateProvider>();

builder.Services.AddAuthorizationCore();

await builder.Build().RunAsync();
