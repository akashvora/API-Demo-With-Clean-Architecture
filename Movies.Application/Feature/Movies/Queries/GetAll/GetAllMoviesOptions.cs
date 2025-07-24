using Movies.Shared.Enums;
using Movies.Shared.Models.Paging;

namespace Movies.Application.Feature.Movies.Queries.GetAll
{
	public class GetAllMoviesOptions
	{
		public required string? Title { get; init; }
		public required int? Year { get; init; }
		public required Guid? UserId { get; init; }
		public required string? SortField { get; init; }
		public required SortOrder SortOrder { get; init; } = SortOrder.Unsorted;
		public PageRequest Paging { get; init; } = new(); // ✅ encapsulated!


	}
}
