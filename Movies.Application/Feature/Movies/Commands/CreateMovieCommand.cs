using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Movies.Application.Feature.Movies.Commands
{
	public class CreateMovieCommand
	{
		public Guid Id { get; set; } = Guid.NewGuid();
		public string Title { get; set; } = default!;
		public int YearOfRelease { get; set; }
		public List<string> Genres { get; set; } = new();
		public string Slug { get; set; }
	}
}
