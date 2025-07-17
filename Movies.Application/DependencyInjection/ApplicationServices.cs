using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Movies.Application.Feature.Authentication.UsesCases;
using Movies.Application.Feature.Movies.UseCases;
using Movies.Application.Feature.Rating.UseCases;

namespace Movies.Application.DependencyInjection
{
	public static class ApplicationServices
	{
		public static IServiceCollection AddApplicationServices(this IServiceCollection services) 
		{ 
			services.AddScoped<CreateMovieUseCase>();
			services.AddScoped<GetMovieByIdOrSlugUseCase>();
			services.AddScoped<GetAllMoviesUseCase>();
			services.AddScoped<UpdateMovieUseCase>();
			services.AddScoped<DeleteMovieUseCase>();
			services.AddValidatorsFromAssemblyContaining<IApplicationMarker>(ServiceLifetime.Scoped);
			services.AddScoped<LoginUseCase>();
			services.AddScoped<RatingUseCase>();
			services.AddScoped<DeleteRatingUseCase>();
			services.AddScoped<GetRatingsForUserUseCase>();
			return services;
		}
	}
}
