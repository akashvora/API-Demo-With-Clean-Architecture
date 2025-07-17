using FluentValidation;
using Movies.Application.Feature.Movies.Commands;
using Movies.Application.Feature.Movies.Interfaces;
using Movies.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movies.Application.Validatiors
{

	public class UpdateMovieCommandValidator: AbstractValidator<UpdateMovieCommand>
	{
		private readonly IMovieRepository _movieRepository;

		public UpdateMovieCommandValidator(IMovieRepository movieRepository)
		{
			_movieRepository = movieRepository;

			RuleFor(Movie => Movie.Id).NotEmpty().WithMessage("Movie ID is required.");
			RuleFor(movie => movie.Genres)
				.NotEmpty().WithMessage("At least one genre is required.")
				.Must(genres => genres.Count <= 5).WithMessage("A maximum of 5 genres is allowed.");
			RuleFor(movie => movie.Title)
				.NotEmpty().WithMessage("Movie title is required.")
				.MaximumLength(100).WithMessage("Movie title must not exceed 100 characters.");
			RuleFor(movie => movie.YearOfRelease)
				.LessThanOrEqualTo(DateTime.Now.Year).WithMessage("Year of release cannot be in the future.");
			RuleFor(movie => movie.Slug)
				.MustAsync(ValidateSlug)
				.WithMessage("This movie already exists in the system");
		}
		private async Task<bool> ValidateSlug(UpdateMovieCommand movie, string slug, CancellationToken cancellationToken = default)
		{
			var existingMovie = await _movieRepository.GetBySlugAsync(slug);
			if (existingMovie.Value is not null)
			{
				return existingMovie.Value.Id == movie.Id; // If there's an error, we assume the slug is valid
			}
			return existingMovie.Value is null; // If there's no existing movie with the same slug, it's valid
		}
	}
}
