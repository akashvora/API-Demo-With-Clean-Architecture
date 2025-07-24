using FluentValidation;
using Movies.Shared.Constants;
using Movies.Shared.Models.Paging;

namespace Movies.Shared.Validation.Paging;

public class PageRequestValidator : AbstractValidator<PageRequest>
{
	public PageRequestValidator()
	{
		RuleFor(x => x.Page)
			.GreaterThanOrEqualTo(1)
			.WithMessage("Page must be at least 1.");

		RuleFor(x => x.PageSize)
			.InclusiveBetween(1, PagingDefaults.MaxPageSize)
			.WithMessage($"PageSize must be between 1 and {PagingDefaults.MaxPageSize}.");
	}
}
