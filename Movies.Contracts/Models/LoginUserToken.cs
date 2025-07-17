using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movies.Domain.Models
{
	public class LoginUserToken
	{
		public string UserId { get; set; } = string.Empty;
		public string Email { get; set; } = string.Empty;
		public string Token { get; set; } = string.Empty;
		public IEnumerable<string> Roles { get; set; } = Enumerable.Empty<string>();
	}
}
