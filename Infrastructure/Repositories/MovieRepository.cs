using Dapper;
using Infrastructure.DataAccessModels;
using Movies.Application.Common;
using Movies.Application.Common.Exceptions;
using Movies.Application.Common.Interfaces;
using Movies.Application.Database;
using Movies.Application.Feature.Movies.Interfaces;
using Movies.Application.Feature.Movies.Queries.GetAll;
using Movies.Domain.Models;
using Movies.Shared.Enums;
using Movies.Shared.Models.Paging;
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
                    from movies m left join ratings r on m.id = r.movieId left join ratings myr on m.id = myr.movieId and myr.userid = @UserId
                    where m.id = @Id group by m.id , m.slug, m.title, m.yearofrelease,myr.rating 
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
                     left join ratings myr on m.id = myr.movieId
                     and myr.userid = @UserId
                    where m.slug = @Slug
                    group by m.id, m.slug, m.title, m.yearofrelease, myr.rating
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


		public async Task<Result<Movies.Shared.Models.Paging.PagedResult<Movie>>> GetAllAsync(GetAllMoviesOptions options, CancellationToken token = default)
		{
			//throw new NotImplementedException();
			////return Task.FromResult(_movies.AsEnumerable());
			//////return Task.FromResult<IEnumerable<Movie>>(_movies);


			using var connection = await _dbConnectionFactory.CreateConnectionAsync(token);

			//var orderClause = string.Empty;
			//if (orderClause is not null)
			//{
			//	orderClause = $"""
			//		,m.{options.SortField} 
			//		order by m.{options.SortField} {(options.SortOrder == SortOrder.Ascending ? "asc" : "desc")}
			//		""";
			//}

			var groupByClause = "m.id, m.slug, m.title, m.yearofrelease, myr.rating";
			if (options.SortField is not null && !groupByClause.Contains(options.SortField))
				groupByClause += $", m.{options.SortField}";

			var orderByClause = options.SortField is null
				? ""
				: $"ORDER BY m.{options.SortField} {(options.SortOrder == SortOrder.Ascending ? "ASC" : "DESC")}";

			var sql = $"""
				-- Total Count Query
				SELECT COUNT(DISTINCT m.id)
				FROM movies m
				LEFT JOIN genres g ON m.id = g.movieid
				WHERE (@title IS NULL OR m.title ILIKE ('%' || @title || '%'))
				  AND (@year IS NULL OR m.yearofrelease = @year);

				-- Paged Data Query
				SELECT m.id, m.title, m.yearofrelease,
					   STRING_AGG(g.name, ',') AS genres,
					   ROUND(AVG(r.rating), 1) AS rating,
					   myr.rating AS userrating
				FROM movies m
				LEFT JOIN genres g ON m.id = g.movieid
				LEFT JOIN ratings r ON m.id = r.movieId
				LEFT JOIN ratings myr ON m.id = myr.movieId AND myr.userid = @UserId
				WHERE (@title IS NULL OR m.title ILIKE ('%' || @title || '%'))
				  AND (@year IS NULL OR m.yearofrelease = @year)
				GROUP BY {groupByClause}
				{orderByClause}
				LIMIT @PageSize OFFSET @Skip;
			""";

			var parameters = new
			{
				UserId = options.UserId,
				title = options.Title,
				year = options.Year,
				PageSize = options.Paging.PageSize,
				Skip = options.Paging.Skip
			};

			var reader = await connection.QueryMultipleAsync(new CommandDefinition(sql, parameters, cancellationToken: token));

			var totalCount = reader.ReadSingle<int>();
			var movies = reader.Read<MovieRaw>().Select(x => new Movie
			{
				Id = x.id,
				Title = x.title,
				YearOfRelease = x.yearofrelease,
				Genres = x.genres?.Split(',').ToList() ?? new List<string>(),
				Rating = (float?)x.rating,
				UserRating = (int?)x.userrating
			}).ToList();

			var pagedResult = new PagedResult<Movie>
			{
				Items = movies,
				Paging = new PagingMetadata
				{
					Page = options.Paging.Page,
					PageSize = options.Paging.PageSize,
					TotalCount = totalCount
				}
			};

			return Result<PagedResult<Movie>>.Success(pagedResult);
			//return Result<PagedResult<Movie>>.Success();
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
