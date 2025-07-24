using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movies.Shared.Models.HyperMedia
{
	public class Link
	{
		public  string Href { get; set; } = default!;
		public  string Rel { get; set; } = default!;
		public  string Method { get; set; } = "GET";

		public Link(string href, string rel, string method = "GET")
		{
			Href = href;
			Rel = rel;
			Method = method;
		}
	}
}