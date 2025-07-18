using Movies.Application.Feature.Movies.Commands;
using Movies.Application.Feature.Rating.Commands.Delete;
using Movies.Application.Feature.Rating.Interfaces;

namespace Movies.Application.Feature.Rating.UseCases.Delete
{
	public class DeleteRatingUseCase
	{
		private readonly IRatingRepository ratingRepository;
		public DeleteRatingUseCase(IRatingRepository _ratingRepository)
		{
			ratingRepository = _ratingRepository;
		}

		public async Task<bool> InvokeAsync(DeleteRatingCommand deleteRatingCommand,CancellationToken cancellationToken) 
		{
			return await ratingRepository.DeleteRatingAsync(deleteRatingCommand.movieId,deleteRatingCommand.userId, cancellationToken:cancellationToken);		
		}
	}
}
