using Movies.Api.Models.Models.Requests;
using Movies.Api.Models.Requests;
using Movies.Api.Models.Responses;
using Refit;

namespace Movies.Api.SDK
{
	[Headers("Authorization: Bearer")]
	public interface IMoviesApi
	{
		[Get(Movies.Api.SDK.ApiEndpoints.Movies.Get)]
		Task<MovieResponse> GetMovieAsync(string idOrSlug);

		[Get(Movies.Api.SDK.ApiEndpoints.Movies.GetAll)]
		Task<MoviesResponse> GetMoviesAsync(GetAllMoviesRequest request);

		[Post(Movies.Api.SDK.ApiEndpoints.Movies.Create)]
		Task<MovieResponse> CreateMovieAsync(CreateMovieRequest request);

		[Put(Movies.Api.SDK.ApiEndpoints.Movies.Update)]
		Task<MovieResponse> UpdateMovieAsync(Guid id, UpdateMovieRequest request);

		[Delete(Movies.Api.SDK.ApiEndpoints.Movies.Delete)]
		Task DeleteMovieAsync(Guid id);

		[Put(Movies.Api.SDK.ApiEndpoints.Movies.Rate)]
		Task RateMovieAsync(Guid id, RateMovieRequest request);

		[Delete(Movies.Api.SDK.ApiEndpoints.Movies.DeleteRating)]
		Task DeleteRatingAsync(Guid id);

		[Get(Movies.Api.SDK.ApiEndpoints.Ratings.GetUserRatings)]
		Task<IEnumerable<MovieRatingResponse>> GetMovieRatingAsync();
	}
}
