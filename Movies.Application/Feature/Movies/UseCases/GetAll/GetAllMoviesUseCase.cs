using AutoMapper;
using FluentValidation;
using Microsoft.IdentityModel.Tokens;
using Movies.Application.Common;
using Movies.Application.Feature.Movies.Interfaces;
using Movies.Application.Feature.Movies.Queries.GetAll;
using Movies.Domain.Models;
using Movies.Shared.Models.Paging;
using Movies.Shared.Enums;
using Movies.Shared.Utilities;

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

		public async Task<Result<PagedResult<Movie>>> ExecuteAsync(GetAllMoviesQuery query, CancellationToken token = default)
		{

			//var options = _mapper.Map<GetAllMoviesOptions>(query);

			var options = query.ToOptions(); // ToOptions static extenstion method handles sorting internally    var skip = (query.Page - 1) * query.PageSize;

			//await _validator.ValidateAndThrowAsync(options,token); // do not validate options here, validate it on controller
			//(string? sortField, SortOrder sortOrder) = SortParser.ParseSortBy(query.SortBy);
			// "SortParser.ParseSortBy(query.SortBy)" will return result: (null, SortOrder.Unsorted) if SortBy file will be empty or null or whitespace

			//options.SortField = sortField;
			//options.SortOrder = sortOrder;

			return await _movieRepository.GetAllAsync(options, token);
		}
	}
}
