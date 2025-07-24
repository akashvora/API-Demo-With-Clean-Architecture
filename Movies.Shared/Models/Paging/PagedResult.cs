using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movies.Shared.Models.Paging
{
	public class PagedResult<T>
	{
		public IReadOnlyList<T> Items { get; init; } = new List<T>();
		public PagingMetadata Paging { get; init; } = new();
	}

}
