using Movies.Api.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace Movies.Api.ApiDepedencyInjection;

public static class AddControllerFilterExtensions
{
	public static IMvcBuilder AddApiFilters(this IMvcBuilder builder)
	{
		builder.AddMvcOptions(options =>
		{
			options.Filters.Add<ApiCustomRequestValidationFilter>();

			// Add more filters here...
			// options.Filters.Add<MyOtherCustomFilter>();
			// options.Filters.Add<LoggingFilter>();
		});

		return builder;
	}
}