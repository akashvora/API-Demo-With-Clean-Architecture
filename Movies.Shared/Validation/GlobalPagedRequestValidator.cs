using FluentValidation;
using Movies.Shared.Constants;
using Movies.Shared.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movies.Shared.Validation
{
	public class GlobalPagedRequestValidator<T> : AbstractValidator<T> where T : IPagedRequest
	{
		public GlobalPagedRequestValidator()
		{
			RuleFor(x => x.Page).GreaterThanOrEqualTo(1)
					 .WithMessage("Page must be at least 1.");

			RuleFor(x => x.PageSize)
			.InclusiveBetween(PagingDefaults.MinPageSize, PagingDefaults.MaxPageSize)
			.WithMessage($"PageSize must be between {PagingDefaults.MinPageSize} and {PagingDefaults.MaxPageSize}.");
		}
	}

}
