using Movies.Application.Feature.Rating.Commands;
using Movies.Application.Feature.Rating.Interfaces;
using Movies.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movies.Application.Feature.Rating.UseCases
{
	public class GetRatingsForUserUseCase
	{
		private readonly IRatingRepository _ratingRepository;
		public GetRatingsForUserUseCase(IRatingRepository ratingRepository)
		{
			_ratingRepository = ratingRepository;
		}

		public async Task<IEnumerable<MovieRating>> InvokeAsync(GetRatingsForUserCommand command, CancellationToken cancellationToken) 
		{
			return await _ratingRepository.GetRatingsForUserAsync(command.userId, cancellationToken);	
		}
	}
}
