using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movies.Application.Common.Interfaces
{
	public interface IDbConnectionFactory
	{
		Task<IDbConnection> CreateConnectionAsync(CancellationToken token = default);
	}
}
