using FluentValidation;
using FluentValidation.AspNetCore;
using Movies.Api.Models.Requests.Validation;
using Microsoft.Extensions.DependencyInjection;


namespace Movies.Api.ApiDepedencyInjection;

public static class AddRequestValidationExtension
{
	public static IServiceCollection AddApiRequestValidations(this IServiceCollection services)
	{
		//services.AddValidatorsFromAssemblyContaining<GetAllMoviesRequestValidator>();

		//return services;
		
		services.AddControllers()
		.AddFluentValidation(fv =>
		{
			fv.RegisterValidatorsFromAssemblyContaining<GetAllMoviesRequestValidator>();
		});

		return services;

	}
}
