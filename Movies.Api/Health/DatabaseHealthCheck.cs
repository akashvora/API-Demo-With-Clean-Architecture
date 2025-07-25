using Microsoft.Extensions.Diagnostics.HealthChecks;
using Movies.Application.Common.Interfaces;

namespace Movies.Api.Health
{
	public class DatabaseHealthCheck : IHealthCheck
	{
		public const string Name = "Database";
		private readonly IDbConnectionFactory _dbConnectionFactory;
		private readonly ILogger<DatabaseHealthCheck> _logger;
		public DatabaseHealthCheck(IDbConnectionFactory dbConnectionFactory)
		{
			_dbConnectionFactory = dbConnectionFactory ?? throw new ArgumentNullException(nameof(dbConnectionFactory));
		}
		public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
		{
			try {
				_ = await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);
				return HealthCheckResult.Healthy("Database connection is healthy.");
			}
			catch (Exception ex)
			{
				const string errorMessage = "Database is unhealthy.";
				_logger?.LogError(ex, errorMessage);
				return HealthCheckResult.Unhealthy(errorMessage);
			}
		}
	}
}
