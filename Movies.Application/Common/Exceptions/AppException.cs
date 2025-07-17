using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movies.Application.Common.Exceptions
{
	public abstract class AppException : Exception
	{
		public int StatusCode { get; }

		protected AppException(string message, int statusCode = 500) : base(message)
		{
			StatusCode = statusCode;
		}
	}
}
