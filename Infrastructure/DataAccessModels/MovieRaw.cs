using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.DataAccessModels
{
	public class MovieRaw
	{
		public Guid id { get; set; }
		public string title { get; set; } = string.Empty;
		public int yearofrelease { get; set; }
		public string? genres { get; set; }
		public float? rating { get; set; }
		public int? userrating { get; set; }
	}

}
