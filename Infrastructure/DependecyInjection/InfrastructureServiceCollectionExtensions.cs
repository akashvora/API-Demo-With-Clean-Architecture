using Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Movies.Application.Common.Interfaces;
using Movies.Application.Database;
using Movies.Application.Feature.Movies.Interfaces;
using Movies.Application.Feature.Rating.Interfaces;
using Movies.Infrastructure.Database;

namespace Infrastructure.DependecyInjection
{
	public static class InfrastructureServiceCollectionExtensions
	{
		public static IServiceCollection AddInfrastutureServices(this IServiceCollection services)
		{
			// Ensure MovieRepository implements IMovieRepository
			services.AddScoped<IMovieRepository, MovieRepository>();
			services.AddScoped<IRatingRepository, RatingRepository>();
			return services;
		}

		public static IServiceCollection AddDatabase(this IServiceCollection services, string connectionString)
		{
			// ✅ Changed from Singleton ➝ Scoped to prevent stale connections
			services.AddTransient<IDbConnectionFactory>(_ => new NpgsqlConnectionFactory(connectionString));
			services.AddScoped<DbInitializer>();

			return services;
		}
	}
}
