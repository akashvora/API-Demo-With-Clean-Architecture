using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Movies.Api.Common.AuthenticationEnums;
using Movies.Api.Common.Extensions;
using Movies.Api.Mapping;
using Movies.Api.Models.Requests;
using Movies.Application.Feature.Movies.Commands;
using Movies.Application.Feature.Movies.Queries;
using Movies.Application.Feature.Movies.UseCases;
using Movies.Application.Feature.Rating.Interfaces;


namespace Movies.Api.Controllers
{
	
	[ApiController]
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
		public MoviesController(//IMovieRepository movieRepository,
			CreateMovieUseCase createMovieUseCase, GetMovieByIdOrSlugUseCase getMovieByIdOrSlugUseCase,UpdateMovieUseCase updateMovieUseCase, GetAllMoviesUseCase getAllMoviesUseCase, DeleteMovieUseCase deleteMovieUseCase, IRatingRepository ratingRepository)
		{
			//_movieRepository = movieRepository;
			_createMovieUseCase = createMovieUseCase;
			_getMovieByIdOrSlugUseCase = getMovieByIdOrSlugUseCase;
			_updateMovieUseCase = updateMovieUseCase;
			_getAllMoviesUsecase = getAllMoviesUseCase;
			_deleteMovieUseCase = deleteMovieUseCase;
			_ratingRepository = ratingRepository;
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

			var result  = await _createMovieUseCase.ExecuteAsync(command, token);

			if (result.IsFailure)
				return StatusCode(result.Problem!.Status ?? 500, result.Problem);

			return CreatedAtAction(nameof(Get), new { idOrSlug = result.Value?.Id }, result.Value);

		}

		[HttpGet(ApiEndpoints.Movies.Get)]
		public async Task<IActionResult> Get([FromRoute] string idOrSlug, CancellationToken token) 
		{
			var query = new GetMovieByIdOrSlugQuery { IdOrSlug=idOrSlug};
			var userId = HttpContext.GetUserId();
			var movie = await _getMovieByIdOrSlugUseCase.ExecuteAsync(query,userId, token);
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
		public async Task<IActionResult> GetAll(CancellationToken token)
		{
			var query = new GetAllMoviesQuery { };
			//var movies = await _movieRepository.GetAllAsync();
			var userId = HttpContext.GetUserId();
			var movies = await _getAllMoviesUsecase.ExecuteAsync(query, userId, token);
			var movieResponse = movies.Value?.MapToResponse();
			return Ok(movieResponse);
		}

		[Authorize(Policy = Policies.AdminOnly)]
		[HttpPut(ApiEndpoints.Movies.Update)]
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
