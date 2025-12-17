namespace Infrastructure
{
	using Application.Common.Services.Data;
	using Microsoft.Extensions.Configuration;
	using Microsoft.Extensions.DependencyInjection;

	public static class ConfigureServices
	{
		public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
		{
			//TODO: Move the DB config there. Example:
			//var connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

			//services.AddDbContext<ApplicationDbContext>(options =>
			//{
			//	options.UseSqlServer(connectionString)
			//		.EnableDetailedErrors() // Enable detailed errors (optional)
			//		.EnableSensitiveDataLogging(); // Enable sensitive data logging (optional)

			//	options.AddInterceptors(new SqlQueryInterceptor());
			//});

			services.AddScoped<IApplicationDbContext>(sp =>
				sp.GetRequiredService<ApplicationDbContext>());

			return services;
		}
	}
}
