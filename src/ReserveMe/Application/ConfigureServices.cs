namespace Application
{
	using System.Reflection;
	using MediatR;
	using Microsoft.Extensions.DependencyInjection;

	public static class ConfigureServices
	{
		public static IServiceCollection AddApplicationServices(this IServiceCollection services)
		{
			services.AddMediatR(cfg =>
			{
				cfg.RegisterServicesFromAssembly(typeof(ConfigureServices).Assembly);
			});

			return services;
		}
	}
}
