using Dapper;
using Microsoft.AspNetCore.Mvc;
using Movies.Application.Common;
using Movies.Application.Common.Exceptions;
using Movies.Application.Common.Interfaces;
using Movies.Application.Database;
using Movies.Application.Feature.Movies.Interfaces;
using Movies.Application.Feature.Movies.Queries.GetAll;
using Movies.Domain.Models;
using Npgsql;

namespace Infrastructure.Repositories
{
	public class MovieRepository : IMovieRepository
	{
		//private readonly List<Movie> _movies = new();
		private readonly IDbConnectionFactory _dbConnectionFactory;
		public MovieRepository(IDbConnectionFactory dbConnectionFactory)
		{
			_dbConnectionFactory = dbConnectionFactory ?? throw new ArgumentNullException(nameof(dbConnectionFactory));
		}

		public async Task<Result<Movie>> CreateAsync(Movie movie, CancellationToken token = default)
		{
			using var connection = await _dbConnectionFactory.CreateConnectionAsync(token);
			using var transaction = connection.BeginTransaction();

			try
			{
				var result = await connection.ExecuteAsync(new CommandDefinition("""
				insert into movies(id,slug,title,yearofrelease)
				values(@Id,@Slug,@Title,@YearOfRelease)
				""", movie, cancellationToken:token));
				if (result > 0)
				{
					foreach (var gener in movie.Genres)
					{
						await connection.ExecuteAsync(new CommandDefinition("""
						insert into genres(movieId,name)
						values(@MovieId,@Name)
						""", new { MovieId = movie.Id, Name = gener }, cancellationToken:token));
					}

					transaction.Commit();
					return Result<Movie>.Success(movie);
				}


				// result > 0 ? movie : null;
				transaction.Rollback();
				return Result<Movie>.Failure("Creation Failed", "Could not persist movie to the database.", 400);

			}
			catch (PostgresException ex) when (ex.SqlState == "23505")
			{
				transaction.Rollback();
				//throw new ConflictException("Movie or genre already exists.");
				return Result<Movie>.Failure("Conflict", "Movie or genre already exists.", 409);
			}
			catch(Exception ex)
			{
				transaction.Rollback();
				return Result<Movie>.Failure("Internal Server Error", ex.Message, 500);

				//throw;
			}

			//_movies.Add(movie);
			//return Task.FromResult(true);
		}
		public async Task<Result<Movie>> GetByIdAsync(Guid id, Guid? userId = default, CancellationToken token = default)
		{
			using var connection = await _dbConnectionFactory.CreateConnectionAsync(token);

			try
			{
				// Query the movie details
				var movie = await connection.QuerySingleOrDefaultAsync<Movie>(
				new CommandDefinition("""
                    select m.id, m.slug, m.title, m.yearofrelease,round(avg(r.rating)) as rating,myr.rating as userrating
                    from movies m left join ratings r on m.id = r.movieId left join ratings myr on myr.id = myr.movieId and myr.userid = @UserId
                    where m.id = @Id
                    """, new { Id = id, UserId=userId }, cancellationToken: token));

				if (movie is null)
				{
					return Result<Movie>.Failure("Not Found", $"Movie with ID '{id}' was not found.", 404);
				}

				// Query the genres associated with the movie
				var genres = await connection.QueryAsync<string>(
					new CommandDefinition("""
                    select g.name
                    from genres g
                    where g.movieId = @Id
                    """, new { Id = id }, cancellationToken: token));

				// Assign the genres to the movie object
				if (genres is not null)
					movie.Genres.AddRange(genres.ToList());
			
				return Result<Movie>.Success(movie);
			}
			catch (Exception ex)
			{
				return Result<Movie>.Failure("Internal Server Error", ex.Message, 500);
				//throw;
			}

		}

		public async Task<Result<Movie>> GetBySlugAsync(string slug, Guid? userId = default, CancellationToken token = default)
		{
			//throw new NotImplementedException();
			////var movie = _movies.SingleOrDefault(m => m.Slug == slug);
			////return Task.FromResult(movie);

			using var connection = await _dbConnectionFactory.CreateConnectionAsync(token);

			// Query the movie details
			var movie = await connection.QuerySingleOrDefaultAsync<Movie>(
				new CommandDefinition("""
                    select m.id, m.slug, m.title, m.yearofrelease,round(avg(r.rating),1) as rating,myr.rating as userrating
                     from movies m
                     left join ratings r on m.id = r.movieId
                     left join ratings myr on myr.id = myr.movieId
                     and myr.userid = @UserId
                    where m.slug = @Slug
                    """, new { Slug = slug, UserId=userId }, cancellationToken: token));

			if (movie is null)
			{
				return Result<Movie>.Failure("Not Found", $"Movie not found.", 404);
			}

			// Query the genres associated with the movie
			var genres = await connection.QueryAsync<string>(
				new CommandDefinition("""
                    select g.name
                    from genres g
                    where g.movieId = @Id
                    """, new { Id = movie.Id }, cancellationToken: token));

			// Assign the genres to the movie object
			if (genres is not null)
				movie.Genres.AddRange(genres.ToList());

			return Result<Movie>.Success(movie);
		}


		public async Task<Result<IEnumerable<Movie>>> GetAllAsync(GetAllMoviesOptions options, CancellationToken token = default)
		{
			//throw new NotImplementedException();
			////return Task.FromResult(_movies.AsEnumerable());
			//////return Task.FromResult<IEnumerable<Movie>>(_movies);


			using var connection = await _dbConnectionFactory.CreateConnectionAsync(token);

			// Query the movie details
			var result = await connection.QueryAsync(
				new CommandDefinition("""
					select  m.id, m.slug, m.title, m.yearofrelease, string_agg(g.name, ',') as genres, 
					round(avg(r.rating),1) as rating,myr.rating as userrating
					from movies m
					left join genres g on m.id = g.movieid
					left join ratings r on m.id = r.movieId
					left join ratings myr on m.id = myr.movieId
					and myr.userid = @UserId
					where (@title is null or m.title like ('%' || @title || '%'))
					and (@yearofrelease is null or m.yearofrelease = @yearofrelease)
					group by m.id, m.slug, m.title, m.yearofrelease,myr.rating
					""", 
					new {
						UserId = options.UserId,
						title= options.Title,
						yearofrelease = options.Year
					}, cancellationToken:token));

			return Result<IEnumerable<Movie>>.Success(result.Select(x => new Movie { 
				Id = x.id,
				Title = x.title,
				YearOfRelease = x.yearofrelease,
				Genres = Enumerable.ToList( x.genres.Split(',')),
				Rating = (float?)x.rating,
				UserRating =(int?) x.userrating
			}));
		}

		public async Task<Result<bool>> UpdateAsync(Movie movie, Guid? userId = default, CancellationToken token = default)
		{
			using var connection = await _dbConnectionFactory.CreateConnectionAsync(token);
			using var transaction = connection.BeginTransaction();

			await connection.ExecuteAsync(new CommandDefinition(""""
				delete from genres where movieid=@Id
				"""", new { Id = movie.Id }, cancellationToken: token));

			foreach (var genres in movie.Genres) 
			{
				await connection.ExecuteAsync(new CommandDefinition(
				"""
				Insert into genres (movieId, name)
				Values (@MovieId, @Name)
				""",new { MovieId=movie.Id, Name=genres}, cancellationToken: token));
			}


			//var sql = """
			//	INSERT INTO genres (movieId, name)
			//	VALUES (@MovieId, @Name)
			//""";
			//var parameters = movie.Genres.Select(name => new { MovieId = movie.Id, Name = name });
			//var command = new CommandDefinition(sql, parameters);
			//await connection.ExecuteAsync(command);

			var result = await connection.ExecuteAsync(new CommandDefinition(
				""""
				update movies set slug=@Slug, title=@Title, yearofrelease=@YearOfRelease
				where id=@Id
				"""", movie,cancellationToken: token));

			transaction.Commit();
			return Result<bool>.Success(result > 0);
			//return result > 0;

			//throw new NotImplementedException();
			////var movieIndex = _movies.FindIndex(m => m.Id == movie.Id);
			////if (movieIndex == -1)
			////{
			////	return Task.FromResult(false);
			////}
			////_movies[movieIndex] = movie;
			////return Task.FromResult(true);
		}

		public async Task<Result<bool>> ExistsByIdAsync(Guid id, CancellationToken token = default)
		{
			//throw new NotImplementedException();
			using var connection = await _dbConnectionFactory.CreateConnectionAsync(token);
			return Result<bool>.Success(
				await connection.ExecuteScalarAsync<bool>(new CommandDefinition("""
				Select count(1) from movies where id=@Id
				""", new { Id=id},cancellationToken:token))
				);
		}
		public async Task<Result<bool>> DeleteByIdAsync(Guid id, CancellationToken token = default)
		{
			using var connection = await _dbConnectionFactory.CreateConnectionAsync(token);
			using var transaction = connection.BeginTransaction();

			await connection.ExecuteAsync(new CommandDefinition(""""
				delete from genres where movieid=@Id
				"""", new { Id = id }, cancellationToken: token));

			var result = await connection.ExecuteAsync(new CommandDefinition(""""
				delete from movies where id=@Id
				"""", new { Id = id }, cancellationToken: token));

			transaction.Commit();
			return Result<bool>.Success(result > 0);
			//return result > 0;
			//throw new NotImplementedException();
			////var removeCount = _movResult<ies.RemoveAll(m => m.Id == id);
			////var MovieRemoved = removeCount > 0;
			////return Task.FromResult(MovieRemoved);
		}

	}
}
