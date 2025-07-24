using Movies.Shared.Models.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movies.Shared.Models.HyperMedia
{
	public class PagedHateoasResponse<T> : LinkResourceBase
	{
		public IEnumerable<T> Items { get; set; } = Enumerable.Empty<T>();
		public PagingMetadata Metadata { get; set; } = default!;
	}

}
