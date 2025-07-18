using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Movies.Application.Feature.Authentication.UsesCases;
using Movies.Application.Feature.Movies.UseCases.Create;
using Movies.Application.Feature.Movies.UseCases.Delete;
using Movies.Application.Feature.Movies.UseCases.GetAll;
using Movies.Application.Feature.Movies.UseCases.GetByIdOrSlug;
using Movies.Application.Feature.Movies.UseCases.Update;
using Movies.Application.Feature.Rating.UseCases.Create;
using Movies.Application.Feature.Rating.UseCases.Delete;
using Movies.Application.Feature.Rating.UseCases.Get;

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
