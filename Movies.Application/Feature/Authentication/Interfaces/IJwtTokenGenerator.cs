using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movies.Application.Feature.Authentication.Interfaces
{
	public interface IJwtTokenGenerator
	{
		string GenerateToken(string userId, string email, IEnumerable<string> roles);
	}
}
