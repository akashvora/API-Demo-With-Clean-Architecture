using FluentValidation;
using Movies.Application.Common;
using Movies.Application.Feature.Movies.Commands;
using Movies.Application.Feature.Movies.Interfaces;
using Movies.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movies.Application.Feature.Movies.UseCases
{
	public class CreateMovieUseCase
	{
		private readonly IMovieRepository _movieRepository;
		private readonly IValidator<CreateMovieCommand> _validator;
		public CreateMovieUseCase(IMovieRepository movieRepository,IValidator<CreateMovieCommand> validator)
		{
			_movieRepository = movieRepository;
			_validator = validator;
		}

		public async Task<Result<Movie>> ExecuteAsync(CreateMovieCommand command, CancellationToken token=default)
		{
			await _validator.ValidateAndThrowAsync(command, token);

			var movie = new Movie
			{
				Id = command.Id,
				Title = command.Title,
				YearOfRelease = command.YearOfRelease,
				Genres = command.Genres,

			};
			var result = await _movieRepository.CreateAsync(movie,token);
			return result;
		}
	}
}
