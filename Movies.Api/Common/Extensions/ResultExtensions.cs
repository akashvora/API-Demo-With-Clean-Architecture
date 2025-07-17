using Microsoft.AspNetCore.Mvc;
using Movies.Application.Common;

namespace Movies.Api.Common.Extensions
{
	public static class ResultExtensions
	{
		private static ProblemDetails FallbackProblemDetails()
		{
			return new ProblemDetails
			{
				Title = "Unknown Error",
				Status = 500,
				Detail = "An unexpected error occurred."
			};
		}

		public static IActionResult ToActionResult<T>(this Result<T> result, ControllerBase controller)
		{
			if (result.IsFailure)
			{
				var problem = result.Problem ?? FallbackProblemDetails();

				return controller.StatusCode(problem.Status ?? 500, problem);
			}

			return controller.Ok(result.Value!);
		}


		public static IActionResult ToCreatedAtAction<T>(
			this Result<T> result,
			ControllerBase controller,
			string actionName,
			object? routeValues,
			Func<T, object>? mapToDto = null)
		{
			if (result.IsFailure)
				return controller.StatusCode(result.Problem?.Status ?? 500, result.Problem);

			var body = mapToDto is null ? result.Value : mapToDto(result.Value!);

			return controller.CreatedAtAction(actionName, routeValues, body);
		}

		public static IActionResult ToNoContentResult<T>(this Result<T> result, ControllerBase controller)
		{
			if (result.IsFailure)
				return controller.StatusCode(result.Problem?.Status ?? 500, result.Problem);

			return controller.NoContent();
		}


	}
}
