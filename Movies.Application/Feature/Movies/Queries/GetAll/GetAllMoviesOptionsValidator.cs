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
		public GetAllMoviesOptionsValidator() 
		{
			RuleFor(x => x.Year).LessThanOrEqualTo(DateTime.UtcNow.Year);
		}
	}
}
