using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movies.Domain.Models
{
	public class MovieRating
	{
		public required Guid MovieId { get; init; }
		public required string Slug { get; init; }
		public required int rating { get; init; }
	}
}
