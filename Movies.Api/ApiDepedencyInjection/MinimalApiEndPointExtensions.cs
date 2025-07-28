using Microsoft.AspNetCore.Routing;
using Movies.Api.MinimalAPIEndPoints.MoviesEndPoints.cs;
using Movies.Api.MinimalAPIEndPoints.RatingsEndPoints.cs;

namespace Movies.Api.ApiDepedencyInjection
{
	public static class MinimalApiEndPointExtensions
	{
		public static IEndpointRouteBuilder MapApiEndpoints(this IEndpointRouteBuilder endpoints)
		{
			endpoints.MapMoviesEndpoints();
			endpoints.MapRatingsEndpoints();
			return endpoints;
		}
	}
}
