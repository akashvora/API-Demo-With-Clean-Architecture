using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Movies.Api.Common.AuthenticationEnums;
using Movies.Api.Common.Extensions;
using Movies.Api.Models.Models.Requests;
using Movies.Api.Models.Responses;
using Movies.Application.Feature.Rating.Commands.Create;
using Movies.Application.Feature.Rating.Commands.Delete;
using Movies.Application.Feature.Rating.Queries.Get;
using Movies.Application.Feature.Rating.UseCases.Create;
using Movies.Application.Feature.Rating.UseCases.Delete;
using Movies.Application.Feature.Rating.UseCases.Get;
using static Movies.Api.ApiEndpoints;

namespace Movies.Api.Controllers
{
	[ApiController]
	[ApiVersion(1.0)]
	public class RatingController : ControllerBase
	{
		private readonly IMapper _mapper;
		private readonly RatingUseCase _ratingUseCase;
		private readonly DeleteRatingUseCase _deleteRatingUseCase;
		private readonly GetRatingsForUserUseCase _getRatingsUserUseCase;
		public RatingController(RatingUseCase ratingUseCase, DeleteRatingUseCase deleteRatingUseCase, GetRatingsForUserUseCase getRatingsUserUseCase, IMapper mapper)
		{
			_ratingUseCase = ratingUseCase;
			_deleteRatingUseCase = deleteRatingUseCase;
			_getRatingsUserUseCase = getRatingsUserUseCase;
			_mapper = mapper;
		}

		[Authorize]
		[HttpPut(ApiEndpoints.Movies.Rate)]
		public async Task<IActionResult> RateMovie([FromRoute] Guid id, [FromBody] RateMovieRequest request, CancellationToken cancellationToken)
		{
			var userId = HttpContext.GetUserId();
			var ratingCommand = new RatingCommand { movieId = id, 
			rating= request.Rating,  userId=userId.GetValueOrDefault()};
			var result_ = await _ratingUseCase.InvokeAsync(ratingCommand,cancellationToken);
			return result_ ? Ok() : BadRequest();
		}

		[Authorize(Policy = Policies.AdminOrTrustMember)]
		[HttpDelete(ApiEndpoints.Movies.DeleteRating)]
		public async Task<IActionResult> DeleteRating([FromRoute] Guid id, CancellationToken cancellationToken)
		{
			var userId = HttpContext.GetUserId();
			var deleteRatingCommand = new DeleteRatingCommand
			{
				movieId = id,
				userId = userId.GetValueOrDefault()
			};
			var result_ = await _deleteRatingUseCase.InvokeAsync(deleteRatingCommand, cancellationToken);
			return result_ ? Ok() : BadRequest();
		}


		[Authorize(Policy = Policies.AdminOrTrustMember)]
		[HttpGet(ApiEndpoints.Ratings.GetUserRatings)]
		public async Task<IActionResult> GetUserRatings(CancellationToken cancellationToken)
		{
			var userId = HttpContext.GetUserId();
			var getRatingsForUserCommand = new GetRatingsForUserQuery
			{
				userId = userId.GetValueOrDefault()
			};
			var ratings = await _getRatingsUserUseCase.InvokeAsync(getRatingsForUserCommand, cancellationToken);
			if (ratings is null || !ratings.Any())
			{
				return Ok(Enumerable.Empty<MovieRatingResponse>());
			}

			//var response = _mapper.Map<MovieRatingResponse>(ratings);
			var response = _mapper.Map<IEnumerable<MovieRatingResponse>>(ratings);
			return Ok(response);
		}
	}
}
