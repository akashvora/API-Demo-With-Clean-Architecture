using Asp.Versioning;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Movies.Api.Common.AuthenticationEnums;
using Movies.Api.Common.Extensions;
using Movies.Api.HyperMedia;
using Movies.Api.Mapping;
using Movies.Api.Models.Requests;
using Movies.Api.Models.Responses;
using Movies.Application.Feature.Movies.Commands.Create;
using Movies.Application.Feature.Movies.Commands.Delete;
using Movies.Application.Feature.Movies.Commands.Update;
using Movies.Application.Feature.Movies.Queries.GetByIdOrSlug;
using Movies.Application.Feature.Movies.UseCases.Create;
using Movies.Application.Feature.Movies.UseCases.Delete;
using Movies.Application.Feature.Movies.UseCases.GetAll;
using Movies.Application.Feature.Movies.UseCases.GetByIdOrSlug;
using Movies.Application.Feature.Movies.UseCases.Update;
using Movies.Application.Feature.Rating.Interfaces;


namespace Movies.Api.Controllers
{

	[ApiController]
	[ApiVersion(1.0)]  // for better practice of versioning version it in different folder wise like V1/Controllers etc; V2/Controllers etc
	[ApiVersion(2.0)]
	[Authorize]
	public class MoviesController : ControllerBase
	{
		//private readonly IMovieRepository _movieRepository;
		private readonly CreateMovieUseCase _createMovieUseCase;
		private readonly UpdateMovieUseCase _updateMovieUseCase;
		private readonly GetAllMoviesUseCase _getAllMoviesUsecase;
		private readonly DeleteMovieUseCase _deleteMovieUseCase;
		private readonly GetMovieByIdOrSlugUseCase _getMovieByIdOrSlugUseCase;
		private readonly IRatingRepository _ratingRepository;  /// this will break the clean architecture. controller should have calling returing logic but in update call doing extra work on controlller
		private readonly ILinkGeneratorService _linkService;
		public MoviesController(//IMovieRepository movieRepository,
			CreateMovieUseCase createMovieUseCase, GetMovieByIdOrSlugUseCase getMovieByIdOrSlugUseCase, UpdateMovieUseCase updateMovieUseCase, GetAllMoviesUseCase getAllMoviesUseCase, DeleteMovieUseCase deleteMovieUseCase, IRatingRepository ratingRepository
			, IValidator<GetAllMoviesRequest> validator, ILinkGeneratorService linkGeneratorService)
		{
			//_movieRepository = movieRepository;
			_createMovieUseCase = createMovieUseCase;
			_getMovieByIdOrSlugUseCase = getMovieByIdOrSlugUseCase;
			_updateMovieUseCase = updateMovieUseCase;
			_getAllMoviesUsecase = getAllMoviesUseCase;
			_deleteMovieUseCase = deleteMovieUseCase;
			_ratingRepository = ratingRepository;
			_linkService = linkGeneratorService;

		}

		//[HttpPost(ApiEndpoints.Movies.Create)]
		//public async Task<IActionResult> Create([FromBody] CreateMovieRequest request)
		//{
		//	//var movie = new Movie
		//	//{
		//	//	Id = Guid.NewGuid(),
		//	//	Title = request.Title,
		//	//	YearOfRelease = request.YearOfRelease,
		//	//	Genres = request.Genres.ToList()
		//	//};
		//	var movie = request.MapToMovie();
		//	await _movieRepository.CreateAsync(movie);
		//	//return Created($"/api/movies/{movie.Id}", movie);
		//	//return Created($"/{ApiEndpoints.Movies.Create}/{movie.Id}", movie);
		//	return CreatedAtAction(nameof(Get),new { idOrSlug = movie.Id }, movie);
		//}


		[Authorize(Policy = Policies.AdminOnly)]

		[HttpPost(ApiEndpoints.Movies.Create)]
		[ProducesResponseType(typeof(MovieResponse), StatusCodes.Status201Created)]
		[ProducesResponseType(typeof(ValidationFailureResponse), StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> Create([FromBody] CreateMovieRequest request, CancellationToken token)
		{
			////var movie = new Movie
			////{
			////	Id = Guid.NewGuid(),
			////	Title = request.Title,
			////	YearOfRelease = request.YearOfRelease,
			////	Genres = request.Genres.ToList()
			////};

			var movie = request.MapToMovie();

			//var movie = request.MapToMovie();
			//await _movieRepository.CreateAsync(movie);
			////return Created($"/api/movies/{movie.Id}", movie);
			////return Created($"/{ApiEndpoints.Movies.Create}/{movie.Id}", movie);
			//return CreatedAtAction(nameof(Get), new { idOrSlug = movie.Id }, movie);

			var command = new CreateMovieCommand
			{
				Title = request.Title,
				YearOfRelease = request.YearOfRelease,
				Genres = request.Genres.ToList(),
				Slug = movie.Slug
			};

			var result = await _createMovieUseCase.ExecuteAsync(command, token);

			if (result.IsFailure)
				return StatusCode(result.Problem!.Status ?? 500, result.Problem);

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

				//var links = result.Value?.GetLinks(routeMap, HttpContext.GetUserId());
				movieResponse.Links = _linkService.GenerateLinks(result.Value, "Movies", routeMap);
				//return CreatedAtAction(nameof(Get), new { idOrSlug = movieResponse.Id }, movieResponse);
			}

			//return CreatedAtAction(nameof(Get), new { idOrSlug = result.Value?.Id }, result.Value);

			return CreatedAtAction(nameof(GetV1), new { idOrSlug = movieResponse.Id }, movieResponse);

		}

		[MapToApiVersion(1.0)]
		[HttpGet(ApiEndpoints.Movies.Get)]
		//[ResponseCache(Duration = 30, VaryByHeader = "Accept, Accept-Encoding", Location = ResponseCacheLocation.Any)]
		[ProducesResponseType(typeof(MovieResponse), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> GetV1([FromRoute] string idOrSlug, CancellationToken token)
		{
			var query = new GetMovieByIdOrSlugQuery { IdOrSlug = idOrSlug };
			var userId = HttpContext.GetUserId();
			var movie = await _getMovieByIdOrSlugUseCase.ExecuteAsync(query, userId, token);
			if (movie == null)
				return NotFound();

			var response = movie.Value?.MapToResponse();
			return Ok(response);

			//var movie = Guid.TryParse(idOrSlug, out var id)?
			//	await _movieRepository.GetByIdAsync(id)
			//	: await _movieRepository.GetBySlugAsync(slug:idOrSlug);
			//if (movie is null) 
			//   {  return NotFound(); }
			//var response =  movie.MapToResponse();
			//return Ok(response);
		}

		[MapToApiVersion(2.0)]
		[HttpGet(ApiEndpoints.Movies.Get)]
		[ResponseCache(Duration = 30, VaryByHeader = "Accept, Accept-Encoding", Location = ResponseCacheLocation.Any)]
		[ProducesResponseType(typeof(MovieResponse), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> GetV2([FromRoute] string idOrSlug, CancellationToken token)
		{
			var query = new GetMovieByIdOrSlugQuery { IdOrSlug = idOrSlug };
			var userId = HttpContext.GetUserId();
			var movie = await _getMovieByIdOrSlugUseCase.ExecuteAsync(query, userId, token);
			if (movie == null)
				return NotFound();

			var response = movie.Value?.MapToResponse();
			return Ok(response);

			//var movie = Guid.TryParse(idOrSlug, out var id)?
			//	await _movieRepository.GetByIdAsync(id)
			//	: await _movieRepository.GetBySlugAsync(slug:idOrSlug);
			//if (movie is null) 
			//   {  return NotFound(); }
			//var response =  movie.MapToResponse();
			//return Ok(response);
		}

		[AllowAnonymous]
		[HttpGet(ApiEndpoints.Movies.GetAll)]
		//[ResponseCache(Duration = 30, VaryByQueryKeys = new []{"title", "year", "sortBy", "page", "pagesize"} ,VaryByHeader = "Accept, Accept-Encoding", Location = ResponseCacheLocation.Any)]
		[ProducesResponseType(typeof(MovieResponse), StatusCodes.Status200OK)]
		public async Task<IActionResult> GetAll([FromQuery] GetAllMoviesRequest request , CancellationToken token)
		{
			//var query = new GetAllMoviesQuery { };
			//var movies = await _movieRepository.GetAllAsync();
			var userId = HttpContext.GetUserId();
			//request.UserId = userId;
			var movies = await _getAllMoviesUsecase.ExecuteAsync(request.ToQuery(userId:userId), token);
			//var movieResponse = movies.Value?.Items?.MapToResponse();
			var movieResponse = movies.Value;
			return Ok(movieResponse);
		}

		[Authorize(Policy = Policies.AdminOnly)]
		[HttpPut(ApiEndpoints.Movies.Update)]
		[ProducesResponseType(typeof(MovieResponse), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateMovieRequest request, CancellationToken token)
		{
			var userId = HttpContext.GetUserId();
			var movie = request.MapToMovie(id);
			var command = new UpdateMovieCommand
			{
				Id=id,
				Title = request.Title,
				YearOfRelease = request.YearOfRelease,
				Genres = request.Genres.ToList(),
				Slug = movie.Slug
			};
			var updated = await _updateMovieUseCase.ExecuteAsync(command,userId, token);

			if (!userId.HasValue)
			{
				var rating = await _ratingRepository.GetRatingAsync(movie.Id, token);
				movie.Rating = rating;
			}

			var ratings = await _ratingRepository.GetRatingAsync(movie.Id, userId.Value, token);
			movie.Rating = ratings.Rating;
			movie.UserRating=ratings.UserRating;
			//var updated = await _movieRepository.UpdateAsync(movie);
			if (!updated.Value)
			{ return NotFound(); }

			var response = movie.MapToResponse();
			return Ok(response);
		}

		[HttpDelete(ApiEndpoints.Movies.Delete)]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken token)
		{
			var command = new DeleteMovieCommand 
			{
				Id = id
			};

			var deleted = await _deleteMovieUseCase.ExecuteAsync(command, token);
			if (!deleted.Value) { 
				return NotFound(); 
			}
			return Ok(deleted);
		}
	}
}
