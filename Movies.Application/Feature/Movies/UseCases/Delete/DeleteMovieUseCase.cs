using Movies.Application.Common;
using Movies.Application.Feature.Movies.Commands.Delete;
using Movies.Application.Feature.Movies.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movies.Application.Feature.Movies.UseCases.Delete
{
	public class DeleteMovieUseCase
	{
		private readonly IMovieRepository _movieRepository;
		public DeleteMovieUseCase(IMovieRepository movieRepository)
		{
			_movieRepository = movieRepository;
		}
		public async Task<Result<bool>> ExecuteAsync(DeleteMovieCommand deleteMovieCommand, CancellationToken token = default)
		{
			return await _movieRepository.DeleteByIdAsync(deleteMovieCommand.Id, token);
		}
	}
}
