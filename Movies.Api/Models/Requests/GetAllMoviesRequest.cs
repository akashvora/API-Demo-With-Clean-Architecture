using Movies.Application.Feature.Movies.Queries.GetAll;
using Movies.Shared.Constants;

namespace Movies.Api.Models.Requests
{
	public class GetAllMoviesRequest
	{
		//public PageRequest Paging { get; init; } = new PageRequest();// this is an alternative way to initialize Paging
		//public PageRequest Paging { get; init; } = new PageRequest(1,10); // this is an alternative way to initialize Paging
		//public PageRequest Paging { get; init; } = new PageRequest
		//{
		//	Page = 1,
		//	PageSize = 10
		//};

		public required int Page { get; init; } = PagingDefaults.DefaultPage;
		public required int PageSize { get; init; } = PagingDefaults.DefaultPageSize;
		public required string? Title { get; init; }
		public required int? Year { get; init; }
		public required string? SortBy { get; init; }

		public GetAllMoviesQuery ToQuery(Guid? userId)
		{
			return new GetAllMoviesQuery
			{
				Title = Title,
				Year = Year,
				SortBy = SortBy,
				UserId = userId,
				//Page = Paging.Page,
				//PageSize = Paging.PageSize
				Page = Page,
				PageSize = PageSize
			};
		}

	}
}
