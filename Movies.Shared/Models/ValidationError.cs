using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movies.Shared.Models
{
	public class ValidationError
	{
		public string PropertyName { get; set; } = default!;
		public string Message { get; set; } = default!;
	}
}
