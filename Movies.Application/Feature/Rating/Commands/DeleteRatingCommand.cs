using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movies.Application.Feature.Rating.Commands
{
	public class DeleteRatingCommand
	{
		public Guid movieId { get; set; }
		public Guid userId { get; set; }
	}
}
