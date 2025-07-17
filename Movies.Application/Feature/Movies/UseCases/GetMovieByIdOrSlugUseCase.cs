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
	public class GetMovieByIdOrSlugUseCase
	{
		private readonly IMovieRepository _movieRepository;
		public GetMovieByIdOrSlugUseCase(IMovieRepository movieRepository)
		{
				_movieRepository = movieRepository;
		}

		public async Task<Result<Movie>> ExecuteAsync(GetMovieByIdOrSlugQuery query, Guid? userId=default, CancellationToken token = default) {
			if (Guid.TryParse(query.IdOrSlug, out var id))
			{ 
				return await _movieRepository.GetByIdAsync(id,userId,token);
			}
			return await _movieRepository.GetBySlugAsync(query.IdOrSlug,userId,token);
		}
	}
}
