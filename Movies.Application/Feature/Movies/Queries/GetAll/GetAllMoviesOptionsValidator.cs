using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movies.Application.Feature.Movies.Queries.GetAll
{
	public class GetAllMoviesOptionsValidator:AbstractValidator<GetAllMoviesOptions>
	{
		private static readonly string[] AcceptableSortFields = 
		{
			"title", "yearofrelease"
		};
		public GetAllMoviesOptionsValidator() 
		{
			//RuleFor(x => x.Year).LessThanOrEqualTo(DateTime.UtcNow.Year);
			//RuleFor(x => x.SortField).Must(x => x is null || AcceptableSortFields.Contains(x, StringComparer.OrdinalIgnoreCase))
			//	.WithMessage("You can only sort by 'title' or yearofrelease'");
		}
	}
}
