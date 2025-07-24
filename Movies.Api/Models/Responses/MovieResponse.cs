using Movies.Shared.Models.HyperMedia;

namespace Movies.Api.Models.Responses
{
	public class MovieResponse:LinkResourceBase
	{
		public  Guid Id { get; init; }
		public  string Title { get; init; } 
		public  string Slug { get; init; }
		public float? Rating { get; init; }
		public int? UserRating { get; init; }
		public  int YearOfRelease { get; init; }
		public  IEnumerable<string> Genres { get; init; } = Enumerable.Empty<string>();
	}
}
