using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movies.Application.Feature.Movies.Commands
{
	public class DeleteMovieCommand
	{
		public Guid Id { get; set; }
	}
}
