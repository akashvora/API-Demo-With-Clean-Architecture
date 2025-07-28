using Movies.Api.MinimalAPIEndPoints.Handlers;

namespace Movies.Api.MinimalAPIEndPoints.MoviesEndPoints.cs
{
	public static class CreateMovieEndPoint
	{
		public const string EndPointName = "CreateMovie";
		public static IEndpointRouteBuilder MapCreateMovie(this IEndpointRouteBuilder endpointRoute)
		{
			endpointRoute.MapPost(ApiEndpoints.Movies.Create, MoviesHandlers.CreateMovieHanlder)
				.WithName(EndPointName);
			return endpointRoute;
		}
	}
}
