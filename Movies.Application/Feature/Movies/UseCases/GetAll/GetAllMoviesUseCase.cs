using AutoMapper;
using FluentValidation;
using Microsoft.IdentityModel.Tokens;
using Movies.Application.Common;
using Movies.Application.Feature.Movies.Interfaces;
using Movies.Application.Feature.Movies.Queries.GetAll;
using Movies.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movies.Application.Feature.Movies.UseCases.GetAll
{
	public class GetAllMoviesUseCase
	{
		private readonly IMovieRepository _movieRepository;
		private readonly IMapper _mapper;
		private readonly IValidator<GetAllMoviesOptions> _validator;

		public GetAllMoviesUseCase(IMovieRepository movieRepository, IMapper mapper, IValidator<GetAllMoviesOptions> validator)
		{
			_movieRepository = movieRepository;
			_mapper = mapper;
			_validator = validator;
		}

		public async Task<Result<IEnumerable<Movie>>> ExecuteAsync(GetAllMoviesQuery query, CancellationToken token = default)
		{

			var options = _mapper.Map<GetAllMoviesOptions>(query);
			await _validator.ValidateAndThrowAsync(options,token);
			return await _movieRepository.GetAllAsync(options, token);
		}
	}
}
