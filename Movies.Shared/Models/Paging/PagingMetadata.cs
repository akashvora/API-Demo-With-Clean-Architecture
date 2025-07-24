using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movies.Shared.Models.Paging
{
	public class PagingMetadata
	{
		public int Page { get; init; }
		public int PageSize { get; init; }
		public int TotalCount { get; init; }

		public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
		public bool HasPreviousPage => Page > 1;
		public bool HasNextPage => Page < TotalPages;
	}
}
