using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Movies.Application.Feature.Movies.Commands
{
	public class UpdateMovieCommand
	{
		public Guid Id { get; set; }
		public string Title { get; set; }
		public int YearOfRelease { get; set; }
		public List<string> Genres { get; set; } = new();
		public string Slug { get; set; }

	}
}
