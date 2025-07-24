using FluentValidation;
using Movies.Shared.Validation;

namespace Movies.Api.Models.Requests.Validation
{
	public class GetAllMoviesRequestValidator : AbstractValidator<GetAllMoviesRequest>
	{
		// will register it globally in Program.cs so need to call it on controller
		private static readonly string[] AcceptableSortFields =
		{
			"title", "yearofrelease"
		};
		public GetAllMoviesRequestValidator()
		{
			RuleFor(x => x.Year).LessThanOrEqualTo(DateTime.UtcNow.Year);
			RuleFor(x => x.SortBy).Must(x => x is null || AcceptableSortFields.Contains(x, StringComparer.OrdinalIgnoreCase))
				.WithMessage("You can only sort by 'title' or yearofrelease'");
			Include(new GlobalPagedRequestValidator<GetAllMoviesRequest>());

			// Other request-specific rules…
		}
	}

}
