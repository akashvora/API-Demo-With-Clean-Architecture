using Movies.Shared.Models.Paging;
using Movies.Shared.Utilities;

namespace Movies.Application.Feature.Movies.Queries.GetAll
{
	public class GetAllMoviesQuery
	{
		public required string? Title { get; init; }
		public required int? Year { get; init; }
		public required Guid? UserId { get; set; }
		public required string? SortBy { get; init; }
		public int Page { get; init; } = 1;
		public int PageSize { get; init; } = 10;

		public GetAllMoviesOptions ToOptions()
		{
			var (sortField, sortOrder) = SortParser.ParseSortBy(SortBy);
			return new GetAllMoviesOptions
			{
				Title = this.Title,
				Year = this.Year,
				UserId = this.UserId,
				SortField = sortField,
				SortOrder = sortOrder,
				Paging = new PageRequest
				{
					Page = this.Page,
					PageSize = this.PageSize
				}
			};
		}
	}

}
