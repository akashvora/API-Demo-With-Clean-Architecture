using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movies.Application.Feature.Movies.Queries.GetAll
{
	public class GetAllMoviesQuery
	{
		public required string? Title { get; init; }
		public required int? Year { get; init; }
		public required Guid? UserId { get; set; }
	}
}
