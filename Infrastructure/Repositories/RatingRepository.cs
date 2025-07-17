using Dapper;
using Movies.Application.Common.Interfaces;
using Movies.Application.Feature.Rating.Interfaces;
using Movies.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
	public class RatingRepository:IRatingRepository
	{
		public readonly IDbConnectionFactory _dbConnectionFactory;
		public RatingRepository(IDbConnectionFactory dbConnectionFactory) {
		_dbConnectionFactory = dbConnectionFactory;
		}

		public async Task<float?> GetRatingAsync(Guid MovieId, CancellationToken cancellationToken)
		{
			using var connection = await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);
			return await connection.QuerySingleOrDefaultAsync<float?>(new CommandDefinition(""""
				select round(avg(r.rating),1) from rating r
				where movieId=@MovieId
				"""",new { MovieId = MovieId},cancellationToken:cancellationToken));
		}

		public async Task<(float? Rating, int? UserRating)> GetRatingAsync(Guid movieId, Guid userId, CancellationToken cancellationToken)
		{
			using var connection = await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);
			return await connection.QuerySingleOrDefaultAsync<(float?,int?)>(new CommandDefinition(""""
				select round(avg(r.rating),1),
				(
				select ratings from ratings
				where movieId=@MovieId 
				and userid=@UserId limit 1
				) 
				from ratings r
				where movieId=@MovieId
				"""", new { UserId=userId,MovieId = movieId }, cancellationToken: cancellationToken));
		}

		public async Task<bool> RateMovieAsync(Guid movieId, int rating, Guid userId, CancellationToken cancellationToken)
		{
			using var connection = await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);
			var result= await connection.ExecuteAsync(new CommandDefinition(""""
				insert into ratings (userid, movieId, rating)
				values (@UserId, @MovieId,@Rating)
				on conflict (userid, movieId) do update
				set rating=@Rating
				"""", new { UserId = userId, MovieId = movieId, Rating=rating }, cancellationToken: cancellationToken));

			return result > 0;
		}

		public async Task<bool> DeleteRatingAsync(Guid movieId, Guid userId, CancellationToken cancellationToken)
		{
			using var connection = await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);
			var result = await connection.ExecuteAsync(new CommandDefinition(""""
				delete from ratings
				where movieId = @MovieId and userid=@UserId
				"""", new { UserId = userId, MovieId = movieId}, cancellationToken: cancellationToken));

			return result > 0;
		}

		public async Task<IEnumerable<MovieRating>> GetRatingsForUserAsync(Guid userId, CancellationToken cancellationToken)
		{
			using var connection = await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);
			return await connection.QueryAsync<MovieRating>(new CommandDefinition(""""
				select r.rating, r.movieId, m.slug
				from ratings r
				inner join movies m on r.movieId=m.id
				where userid=@UserId
				"""", new { UserId = userId}, cancellationToken: cancellationToken));

		}
	}
}
