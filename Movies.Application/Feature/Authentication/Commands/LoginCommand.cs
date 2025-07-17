using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movies.Application.Feature.Authentication.Commands
{
	public class LoginCommand
	{
		public string UserId { get; set; } = string.Empty;
		public string Email { get; set; } = string.Empty;
		public required IEnumerable<string> Roles { get; set; } = Enumerable.Empty<string>()
;
	}
}
