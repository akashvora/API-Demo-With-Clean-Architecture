using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Movies.Shared.Models.HyperMedia
{
	public abstract class LinkResourceBase
	{
		[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
		public List<Link> Links { get; set; } = new();
	}

}
