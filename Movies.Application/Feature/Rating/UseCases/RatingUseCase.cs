using FluentValidation.Results;
using Movies.Application.Feature.Rating.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FluentValidation;
using Movies.Application.Feature.Movies.Interfaces;
using Movies.Application.Feature.Rating.Commands;
namespace Movies.Application.Feature.Rating.UseCases
{
	public class RatingUseCase
	{
		private readonly IRatingRepository _ratingRepository;
		private readonly IMovieRepository _movieRepository;
		public RatingUseCase(IRatingRepository rating, IMovieRepository movieRepository)
		{
			_ratingRepository = rating;
			_movieRepository = movieRepository;
		}

		public async Task<bool> InvokeAsync(RatingCommand command, CancellationToken cancellationToken)
		{
			if (command.rating is <= 0 or > 5)
			{
				throw new FluentValidation.ValidationException(new [] {
					new ValidationFailure
					{
						PropertyName="Rating",
						ErrorMessage = "Rating Must be between 1 and 5"
					}
				});
			}

			var movieExists = await _movieRepository.ExistsByIdAsync(command.movieId, cancellationToken);
			if (!movieExists.Value)
			{
				return false;
			}

			return await _ratingRepository.RateMovieAsync(command.movieId,command.rating,command.userId,cancellationToken);
		}
	}
}
