namespace ReserveMe
{
	using Blazored.LocalStorage;
	using Microsoft.AspNetCore.Components.Web;
	using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
	using Shared.Helpers;
	using Shared.Providers;

	public class Program
	{
		public static async Task Main(string[] args)
		{
			var builder = WebAssemblyHostBuilder.CreateDefault(args);
			builder.RootComponents.Add<App>("#app");
			builder.RootComponents.Add<HeadOutlet>("head::after");

			builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

			builder.Services.AddBlazoredLocalStorage();

			// API provider
			builder.Services.AddScoped<IApiProvider, ApiProvider>();

			// Authentication helper
			builder.Services.AddScoped<IAuthenticationHelper, AuthenticationHelper>();

			await builder.Build().RunAsync();
		}
	}
}
