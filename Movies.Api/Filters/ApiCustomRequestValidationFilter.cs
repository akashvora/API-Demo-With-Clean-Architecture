using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Movies.Api.Filters
{
	public class ApiCustomRequestValidationFilter : IActionFilter
	{
		public void OnActionExecuting(ActionExecutingContext context)
		{
			if (!context.ModelState.IsValid)
			{
				var errors = context.ModelState
					.Where(kvp => kvp.Value.Errors.Count > 0)
					.Select(kvp => new {
						field = kvp.Key,
						message = kvp.Value.Errors.First().ErrorMessage
					});

				context.Result = new BadRequestObjectResult(new
				{
					status = 400,
					errorCode = "VALIDATION_FAILED",
					errors
				});
			}
		}

		public void OnActionExecuted(ActionExecutedContext context) { }
	}

}
