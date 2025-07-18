using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movies.Application.Feature.Rating.Queries.Get
{
	public class GetRatingsForUserQuery
	{
		public required Guid userId { get; set; }
	}
}
