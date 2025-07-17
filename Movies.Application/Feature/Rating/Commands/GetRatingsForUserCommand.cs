using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movies.Application.Feature.Rating.Commands
{
	public class GetRatingsForUserCommand
	{
		public required Guid userId { get; set; }
	}
}
