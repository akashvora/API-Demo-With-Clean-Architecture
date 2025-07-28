using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Movies.Api.Common.Extensions;
using Movies.Api.HyperMedia;
using Movies.Api.Mapping;
using Movies.Api.MinimalAPIEndPoints.MoviesEndPoints.cs;
using Movies.Api.Models.Requests;
using Movies.Api.Models.Responses;
using Movies.Application.Feature.Movies.Commands.Create;
using Movies.Application.Feature.Movies.Queries.GetByIdOrSlug;
using Movies.Application.Feature.Movies.UseCases.Create;
using Movies.Application.Feature.Movies.UseCases.GetAll;
using Movies.Application.Feature.Movies.UseCases.GetByIdOrSlug;
using System.Reflection.Metadata.Ecma335;

namespace Movies.Api.MinimalAPIEndPoints.Handlers
{
	public class MoviesHandlers
	{
	   public static async Task<IResult> GetMovieHandler(
	   string idOrSlug,
	   GetMovieByIdOrSlugUseCase usecase,
	   HttpContext context,
	   CancellationToken token)
		{
			var query = new GetMovieByIdOrSlugQuery { IdOrSlug = idOrSlug };
			var userId = context.GetUserId();
			var movie = await usecase.ExecuteAsync(query, userId, token);
			var response = movie.Value?.MapToResponse();

			return movie is not null
				? TypedResults.Ok(response)
				: TypedResults.NotFound();
		}

		public static async Task<IResult> CreateMovieHanlder(CreateMovieRequest request, 
			CreateMovieUseCase usecase, ILinkGeneratorService _linkService, IOutputCacheStore _outputCacheStore,
			HttpContext context, CancellationToken token) {


			var movie = request.MapToMovie();

			var command = new CreateMovieCommand
			{
				Title = request.Title,
				YearOfRelease = request.YearOfRelease,
				Genres = request.Genres.ToList(),
				Slug = movie.Slug
			};

			var result = await usecase.ExecuteAsync(command, token);
			await _outputCacheStore.EvictByTagAsync("movies", token);

			if (result.IsFailure)
			{
				return Results.Json(result.Problem, statusCode: result.Problem!.Status ?? 500);
				//return TypedResults.StatusCode(result.Problem!.Status ?? 500, result.Problem);
			}

			if (result.IsFailure)
					return TypedResults.Problem(result.Problem!);
				

			MovieResponse movieResponse = new MovieResponse();

			if (result.Value != null)
			{
				movieResponse = result.Value.MapToResponse();
				var routeMap = new Dictionary<string, (string action, string method, string[] paramNames)>
				{
					["self"] = ("Get", "GET", new[] { "idOrSlug" }),                             // e.g. GET /movies/{id}
					["update"] = ("Update", "PUT", new[] { "id" }),                          // e.g. PUT /movies/{id}
					["delete"] = ("Delete", "DELETE", new[] { "id" }),                       // e.g. DELETE /movies/{id}
					["create"] = ("Create", "POST", new string[] { }),                       // e.g. POST /movies
					["getAll"] = ("GetAll", "GET", new[] { "Title", "YearOfRelease", "SortBy", "Page", "PageSize" }),
				};

				movieResponse.Links = _linkService.GenerateLinks(result.Value, "Movies", routeMap);
			}

			//return CreatedAtAction(nameof(Get), new { idOrSlug = result.Value?.Id }, result.Value);

			return TypedResults.CreatedAtRoute(movieResponse, GetMovieEndPoint.EndpointName, new { idOrSlug = result.Value?.Id });
		}

		public static async Task<IResult> UpdateMovieHandler(Guid id, UpdateMovieRequest request, 
			CreateMovieUseCase usecase, IOutputCacheStore _outputCacheStore, HttpContext context, CancellationToken token) 
		{
			var movie = request.MapToMovie(id);
			var command = new CreateMovieCommand
			{
				Id = id,
				Title = request.Title,
				YearOfRelease = request.YearOfRelease,
				Genres = request.Genres.ToList(),
				Slug = movie.Slug
			};
			var result = await usecase.ExecuteAsync(command, token);
			await _outputCacheStore.EvictByTagAsync("movies", token);
			if (result.IsFailure)
				return TypedResults.Problem(result.Problem!);
			return TypedResults.Ok(result.Value?.MapToResponse());
		}
		public static async Task<IResult> GetAllMovieHandler([AsParameters] GetAllMoviesRequest request, GetAllMoviesUseCase useCase, HttpContext context,  CancellationToken token) 
		{
			var userId = context.GetUserId();
			var movies = await useCase.ExecuteAsync(request.ToQuery(userId: userId), token);
			var movieResponse = movies.Value;
			return TypedResults.Ok(movieResponse);
		}

	}
}
