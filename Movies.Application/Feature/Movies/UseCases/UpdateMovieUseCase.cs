using FluentValidation;
using Movies.Application.Common;
using Movies.Application.Feature.Movies.Commands;
using Movies.Application.Feature.Movies.Interfaces;
using Movies.Application.Feature.Rating.Interfaces;
using Movies.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movies.Application.Feature.Movies.UseCases
{
	public class UpdateMovieUseCase
	{
		private readonly IMovieRepository _movieRepository;
		private readonly IRatingRepository _ratingRepository;
		private readonly IValidator<UpdateMovieCommand> _validator;
		public UpdateMovieUseCase(IMovieRepository movieRepository, IRatingRepository ratingRepository, IValidator<UpdateMovieCommand> validator)
		{
			_movieRepository = movieRepository;
			_ratingRepository = ratingRepository;
			_validator = validator;
		}

		public async Task<Result<bool>> ExecuteAsync(UpdateMovieCommand updateMovieCommand, Guid? userId=default, CancellationToken token = default)
		{
			await _validator.ValidateAndThrowAsync(updateMovieCommand,token);
			var movie = new Movie {
			 Id = updateMovieCommand.Id,
			 Title = updateMovieCommand.Title,
			 YearOfRelease = updateMovieCommand.YearOfRelease,
			 Genres = updateMovieCommand.Genres,
			};	
			var MovieExists = _movieRepository.ExistsByIdAsync(movie.Id, token);
			if (MovieExists.Result.Equals(null)) {
				//return false;
				return Result<bool>.Failure("Already Exists", $"Movie already exists.", 409);
			}
			return await _movieRepository.UpdateAsync(movie, userId, token);
			//await _movieRepository.UpdateAsync(movie, userId, token);
			//if (userId.HasValue)
			//{
			//	var rating = await _ratingRepository.GetRatingAsync(movie.Id,token);
			//	movie.Rating = rating;
			//	return movie;
			//}	

		}
	}
}
