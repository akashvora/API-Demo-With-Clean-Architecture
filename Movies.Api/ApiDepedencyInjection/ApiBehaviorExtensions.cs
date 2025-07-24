using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace Movies.Api.DependencyInjection;

public static class ApiBehaviorExtensions
{
	public static IServiceCollection ConfigureApiBehavior(this IServiceCollection services)
	{
		services.Configure<ApiBehaviorOptions>(options =>
		{
			options.InvalidModelStateResponseFactory = context =>
			{
				var errors = context.ModelState
					.Where(e => e.Value.Errors.Count > 0)
					.Select(e => new
					{
						field = e.Key,
						message = e.Value.Errors.First().ErrorMessage
					});


				var problemDetails = new
				{
					title = "VALIDATION_FAILED",
					detail = "One or more input validations failed.",
					status = StatusCodes.Status400BadRequest,
					instance = context.HttpContext.Request.Path,
					errors = errors
				};

				return new BadRequestObjectResult(problemDetails);
			};
		});

		return services;
	}
}
