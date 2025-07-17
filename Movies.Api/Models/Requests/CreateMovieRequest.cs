using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movies.Api.Models.Requests
{
	public class CreateMovieRequest
	{
		public required string Title { get; init; } = string.Empty;
		public required int YearOfRelease { get; init; }
		public required IEnumerable<string> Genres { get; init; } = Enumerable.Empty<string>();
	}
}
