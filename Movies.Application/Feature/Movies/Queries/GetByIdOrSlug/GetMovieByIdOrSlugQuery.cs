﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movies.Application.Feature.Movies.Queries.GetByIdOrSlug
{
	public class GetMovieByIdOrSlugQuery
	{
		public string IdOrSlug { get; set; } = default!;
	}
}
