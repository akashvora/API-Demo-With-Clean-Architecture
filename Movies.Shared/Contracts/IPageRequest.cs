using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movies.Shared.Contracts
{
	public interface IPagedRequest
	{
		int Page { get; }
		int PageSize { get; }
	}
}
