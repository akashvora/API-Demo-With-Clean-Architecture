using Movies.Application.Common;
using Movies.Application.Feature.Movies.Queries.GetAll;
using Movies.Domain.Models;
using Movies.Shared.Models.Paging;

namespace Movies.Application.Feature.Movies.Interfaces
{
	public interface IMovieRepository
	{
		Task<Result<Movie>> CreateAsync(Movie movie, CancellationToken token = default);
		Task<Result<Movie>> GetByIdAsync(Guid id, Guid? userId = default, CancellationToken token = default);
		Task<Result<Movie>> GetBySlugAsync(string slug, Guid? userId = default, CancellationToken token = default);
		Task<Result<PagedResult<Movie>>> GetAllAsync(GetAllMoviesOptions query, CancellationToken token = default);
		Task<Result<bool>> UpdateAsync(Movie movie, Guid? userId = default, CancellationToken token = default);
		Task<Result<bool>> DeleteByIdAsync(Guid id, CancellationToken token = default);
		Task<Result<bool>> ExistsByIdAsync(Guid id, CancellationToken token = default);
	}
}
