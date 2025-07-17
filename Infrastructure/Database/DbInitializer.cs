using Dapper;
using Movies.Application.Common.Interfaces;

namespace Movies.Infrastructure.Database
{
	public  class DbInitializer
	{
		private readonly IDbConnectionFactory _connectionFactory;
		public DbInitializer(IDbConnectionFactory connectionFactory) { 
			this._connectionFactory=connectionFactory;
		}
		public async Task InitializeAsync()
		{
			using var connection = await _connectionFactory.CreateConnectionAsync();
			using var transaction = connection.BeginTransaction();

			await connection.ExecuteAsync("""
				create table if not exists movies(
				id UUID primary key,
				slug text not null,
				title text not null, 
				yearofrelease integer not null
				)	
				""");

			//await connection.ExecuteAsync("""
			//	drop table if exists genres;
			//""");


			await connection.ExecuteAsync("""
				create table if not exists genres(
				movieId UUID references movies (id),
				name text not null
				)	
				""");

			await connection.ExecuteAsync("""
				create table if not exists ratings(
				userid uuid,
				movieId UUID references movies (id),
				rating integer not null,
				primary key (userid,movieId)
				)	
				""");

			transaction.Commit();

			await connection.ExecuteAsync(sql: """"
				create unique index concurrently if not exists movies_slug_idx
				on movies
				using btree(slug);
				"""");

		}
	}
}
