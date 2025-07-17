using Movies.Application.Common;
using Movies.Application.Feature.Movies.Interfaces;
using Movies.Application.Feature.Movies.Queries;
using Movies.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movies.Application.Feature.Movies.UseCases
{
	public class GetAllMoviesUseCase
	{
		private readonly IMovieRepository _movieRepository;
		public GetAllMoviesUseCase(IMovieRepository movieRepository)
		{
			_movieRepository = movieRepository;
		}

		public async Task<Result<IEnumerable<Movie>>> ExecuteAsync(GetAllMoviesQuery query, Guid? userId=default, CancellationToken token = default)
		{
			return await _movieRepository.GetAllAsync(query, userId,token);
		}
	}
}
