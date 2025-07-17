using Movies.Application.Common.Interfaces;
using Npgsql;
using System.Data;

namespace Movies.Application.Database
{
	public class NpgsqlConnectionFactory : IDbConnectionFactory
	{
		private readonly string _connectionString;	
		public NpgsqlConnectionFactory(string connectionString)
		{
			_connectionString = connectionString;
		}
		public async Task<IDbConnection> CreateConnectionAsync(CancellationToken token = default)
		{
			//var connection = new NpgsqlConnection(_connectionString);
			//await connection.OpenAsync();
			//return connection;
			try
			{
				///host.docker.internal
				//var testConnection = new NpgsqlConnection("Host=host.docker.internal;Port=5433;Database=movies;Username=course;Password=dev@2030");
				//await testConnection.OpenAsync();

				Console.WriteLine($"🔍 Connecting with: {_connectionString}");

				var connection =  new NpgsqlConnection(_connectionString);
				await connection.OpenAsync(token);
				Console.WriteLine("Connection opened successfully!");
				return connection;
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Connection failed: {ex.Message}");
				throw;
			}

		}
	}
}

// Configuration settings for PostgreSQL
// listen_addresses = 'localhost'   # or '*'
// port = 5432
