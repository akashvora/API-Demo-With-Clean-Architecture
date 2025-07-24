using Movies.Shared.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movies.Shared.Models.Paging
{
	public class PageRequest
	{
		public int Page { get; init; } = PagingDefaults.DefaultPage;
		public int PageSize { get; init; } = PagingDefaults.DefaultPageSize;
		public int Skip => (Page - 1) * PageSize;
		public int Take => PageSize;

		public PageRequest(int page = PagingDefaults.DefaultPage, int pageSize = PagingDefaults.DefaultPageSize)
		{
			Page = page;
			PageSize = pageSize;
		}
		//public PageRequest()
		//{
		//	Page = PagingDefaults.DefaultPage;
		//	PageSize = PagingDefaults.DefaultPageSize;
		//}

		// defaul constructor commented out as it is not needed because defualt values are set in the properties
	}
}
