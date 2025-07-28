using Movies.Api.MinimalAPIEndPoints.Handlers;

namespace Movies.Api.MinimalAPIEndPoints.MoviesEndPoints.cs
{
	public static class GetMovieEndPoint
	{
		public const string EndpointName = "GetMovie";
		public static IEndpointRouteBuilder MapGetMovieEndPoint(this IEndpointRouteBuilder endpoints)
		{

			endpoints.MapGet(ApiEndpoints.Movies.Get, MoviesHandlers.GetMovieHandler)
			.WithName(EndpointName);
			//.Produces<MovieDto>()
			//.Produces(404);
			return endpoints;
		}
	}
}
