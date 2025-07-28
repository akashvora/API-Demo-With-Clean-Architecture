using Movies.Api.Common.AuthenticationEnums;
using Movies.Api.MinimalAPIEndPoints.Handlers;

namespace Movies.Api.MinimalAPIEndPoints.MoviesEndPoints.cs
{
	public static class GetAllMoviesEndPoint
	{
		public const string EndPointName = "GetAllMovies";
		public static IEndpointRouteBuilder MapGetAllMovies(this IEndpointRouteBuilder endpointRoute)
		{
			endpointRoute.MapGet(ApiEndpoints.Movies.GetAll, MoviesHandlers.GetAllMovieHandler)
				.WithName(EndPointName).RequireAuthorization(Policies.AdminOrTrustMember);
			return endpointRoute;
		}
	}
}
