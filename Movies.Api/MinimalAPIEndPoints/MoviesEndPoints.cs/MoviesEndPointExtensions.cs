namespace Movies.Api.MinimalAPIEndPoints.MoviesEndPoints.cs
{
	public static class MoviesEndPointExtensions
	{
		public static IEndpointRouteBuilder MapMoviesEndpoints(this IEndpointRouteBuilder endpoints)
		{
			endpoints.MapGetMovieEndPoint();
			endpoints.MapCreateMovie();
			endpoints.MapGetAllMovies();
			return endpoints;
		}
	}
}
