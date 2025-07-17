using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movies.Application.Feature.Rating.Commands
{
	public class RatingCommand
	{
		public Guid movieId {  get; set; } 
		public int rating { get; set; }
		public Guid userId { get; set; }	
	}
}
