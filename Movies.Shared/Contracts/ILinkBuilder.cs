using Movies.Shared.Models.HyperMedia;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movies.Shared.Contracts
{
	public interface ILinkBuilder<T>
	{
		List<Link> BuildLinks(T resource, object? routeValues=null);

	}
}
