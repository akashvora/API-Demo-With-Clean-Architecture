using Movies.Domain.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movies.Application.Feature.Rating.Interfaces
{
	public interface IRatingRepository
	{
		Task<bool> RateMovieAsync(Guid movieId, int rating,Guid userId, CancellationToken cancellationToken=default);
		Task<float?> GetRatingAsync(Guid MovieId, CancellationToken cancellationToken=default);
		Task<(float? Rating, int? UserRating)> GetRatingAsync(Guid movieId, Guid userId, CancellationToken cancellationToken=default);
		Task<bool> DeleteRatingAsync(Guid movieId, Guid userId, CancellationToken cancellationToken=default);
		Task<IEnumerable<MovieRating>> GetRatingsForUserAsync(Guid userId,CancellationToken cancellationToken=default); 
	}
}
